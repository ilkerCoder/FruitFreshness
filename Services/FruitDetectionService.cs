using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing;
using System.Drawing.Imaging;
using Compunet.YoloV8;

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
            // YOLO ile tahminleri al (Bitmap değil, string yol gönderiyoruz)
            var predictions = _yolo.Detect(imagePath);

            using var image = new Bitmap(imagePath);
            using var g = Graphics.FromImage(image);

            int healthy = 0, rotten = 0;

            foreach (var prediction in predictions)
            {
                var bounds = prediction.Bounds;

                var rect = new Rectangle(
                    (int)bounds.X,
                    (int)bounds.Y,
                    (int)bounds.Width,
                    (int)bounds.Height
                );

                if (rect.Width < 10 || rect.Height < 10)
                    continue;

                var crop = new Bitmap(rect.Width, rect.Height);
                using (var cropG = Graphics.FromImage(crop))
                    cropG.DrawImage(image, new Rectangle(0, 0, crop.Width, crop.Height), rect, GraphicsUnit.Pixel);

                var label = ClassifyResNet(crop);
                var color = label == "Healthy" ? Color.Green : Color.Red;

                if (label == "Healthy") healthy++;
                else rotten++;

                g.DrawRectangle(new Pen(color, 2), rect);
                g.DrawString(label, new Font("Arial", 12), new SolidBrush(color), rect.X, rect.Y - 20);
            }


            // Görseli diske kaydet
            var outputPath = Path.Combine("Outputs", "result.jpg");
            Directory.CreateDirectory("Outputs");
            image.Save(outputPath, ImageFormat.Jpeg);

            // Base64'e çevirip dön
            var bytes = File.ReadAllBytes(outputPath);
            var base64 = Convert.ToBase64String(bytes);

            return ($"data:image/jpeg;base64,{base64}", healthy, rotten);
        }


        private string ClassifyResNet(Bitmap input)
        {
            var resized = new Bitmap(input, new Size(224, 224));
            var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });

            for (int y = 0; y < 224; y++)
            {
                for (int x = 0; x < 224; x++)
                {
                    var pixel = resized.GetPixel(x, y);
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
