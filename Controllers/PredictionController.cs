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

        public PredictionController(OnnxPredictionService predictor)
        {
            _predictor = predictor;
        }

        [HttpPost("predict")]
        public async Task<IActionResult> Predict(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            using var stream = image.OpenReadStream();
            var result = _predictor.Predict(stream);
            return Ok(new { prediction = result });
        }
    }
}
