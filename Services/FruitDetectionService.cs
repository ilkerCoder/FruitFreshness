using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Compunet.YoloV8;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;

namespace FruitFreshnessDetector.Services
{
    public class DetectionService
    {
        private readonly InferenceSession _resnetSession;
        private readonly YoloPredictor _yolo;

        public DetectionService(string resnetPath, string yoloPath)
        {
            _resnetSession = new InferenceSession(resnetPath);
            _yolo = new YoloPredictor(yoloPath);
        }

        public (string base64Image, int healthyCount, int rottenCount) PredictFromImage(string imagePath)
        {
            var predictions = _yolo.Detect(imagePath);
            using var image = Image.Load<Rgba32>(imagePath);

            int healthy = 0, rotten = 0;
            var font = SystemFonts.CreateFont("Arial", 12);

            foreach (var prediction in predictions)
            {
                var rect = new Rectangle(
                    (int)prediction.Bounds.X,
                    (int)prediction.Bounds.Y,
                    (int)prediction.Bounds.Width,
                    (int)prediction.Bounds.Height
                );

                if (rect.Width < 10 || rect.Height < 10)
                    continue;

                // Tahmin edilen alanı kırp ve sınıflandır
                using var crop = image.Clone(ctx => ctx.Crop(rect));
                var label = ClassifyResNet(crop);
                var color = label == "Healthy" ? Color.Green : Color.Red;

                if (label == "Healthy") healthy++; else rotten++;

                // Kutuyu çiz, etiket yaz
                image.Mutate(ctx =>
                {
                    ctx.Draw(color, 2, rect);
                    ctx.DrawText(label, font, color, new PointF(rect.X, rect.Y - 20));
                });
            }

            var outputPath = Path.Combine("Outputs", "result.jpg");
            Directory.CreateDirectory("Outputs");
            image.SaveAsJpeg(outputPath);

            var base64 = Convert.ToBase64String(File.ReadAllBytes(outputPath));
            return ($"data:image/jpeg;base64,{base64}", healthy, rotten);
        }

        private string ClassifyResNet(Image<Rgba32> input)
        {
            input.Mutate(x => x.Resize(new Size(224, 224)));

            var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });

            for (int y = 0; y < 224; y++)
            {
                for (int x = 0; x < 224; x++)
                {
                    var pixel = input[x, y];
                    tensor[0, 0, y, x] = (pixel.R / 255f - 0.485f) / 0.229f;
                    tensor[0, 1, y, x] = (pixel.G / 255f - 0.456f) / 0.224f;
                    tensor[0, 2, y, x] = (pixel.B / 255f - 0.406f) / 0.225f;
                }
            }

            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input", tensor) };
            using var results = _resnetSession.Run(inputs);
            var output = results.First().AsEnumerable<float>().ToArray();

            var index = Array.IndexOf(output, output.Max());
            return index == 0 ? "Healthy" : "Rotten";
        }
    }
}
