using FruitFreshnessDetector.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FruitFreshnessDetector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly OnnxPredictionService _predictor;

        private readonly DetectionService _detectionService;

        public PredictionController(DetectionService detectionService, OnnxPredictionService predictionService)
        {
            _predictor = predictionService;
            _detectionService = detectionService;
        }

        [HttpPost("predict")]
        public async Task<IActionResult> Predict(IFormFile singleImage)
        {
            if (singleImage == null || singleImage.Length == 0)
                return BadRequest("No singleImage uploaded.");

            using var stream = singleImage.OpenReadStream();
            var result = _predictor.Predict(stream);
            return Ok(new { prediction = result });
        }

        [HttpPost("detect")]
        public async Task<IActionResult> Detect(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            // Görseli geçici olarak kaydet
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(uploadsDir);

            var imagePath = Path.Combine(uploadsDir, image.FileName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Tespit ve sınıflandırma işlemi
            var (base64Image, healthy, rotten) = _detectionService.PredictFromImage(imagePath);

            return Ok(new
            {
                healthyCount = healthy,
                rottenCount = rotten,
                annotatedImageBase64 = base64Image
            });
        }


    }
}
