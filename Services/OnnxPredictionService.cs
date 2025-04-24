using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing;

namespace FruitFreshnessDetector.Services
{
    public class OnnxPredictionService
    {
        private readonly InferenceSession _session;

        public OnnxPredictionService(string modelPath)
        {
            _session = new InferenceSession(modelPath);
        }

        public string Predict(Stream imageStream)
        {
            return "wip";
            //    //using var image = new Bitmap(imageStream);
            //    //using var resized = new Bitmap(image, new Size(224, 224));

            //    // Tensor oluştur
            //    var input = new DenseTensor<float>(new[] { 1, 3, 224, 224 });

            //    for (int y = 0; y < 224; y++)
            //    {
            //        for (int x = 0; x < 224; x++)
            //        {
            //            var pixel = resized.GetPixel(x, y);
            //            input[0, 0, y, x] = (pixel.R / 255f - 0.485f) / 0.229f;
            //            input[0, 1, y, x] = (pixel.G / 255f - 0.456f) / 0.224f;
            //            input[0, 2, y, x] = (pixel.B / 255f - 0.406f) / 0.225f;
            //        }
            //    }

            //    var inputs = new List<NamedOnnxValue>
            //    {
            //        NamedOnnxValue.CreateFromTensor("input", input)
            //    };

            //    using var results = _session.Run(inputs);
            //    var output = results.First().AsEnumerable<float>().ToArray();

            //    int predictedIndex = Array.IndexOf(output, output.Max());
            //    return predictedIndex == 0 ? "Healthy" : "Rotten";
            //}
        }
    }
}
