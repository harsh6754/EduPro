using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ASPNETCoreIdentityDemo.Services;
using System;

namespace ASPNETCoreIdentityDemo.Controllers
{
    [Route("captcha")]
    public class CaptchaController : Controller
    {
        private readonly IMemoryCache _cache;

        public CaptchaController(IMemoryCache cache)
        {
            _cache = cache;
        }

        // GET /captcha/generate
        [HttpGet("generate")]
        public IActionResult GenerateCaptcha()
        {
            // Generate random CAPTCHA code
            var captchaCode = CaptchaService.GenerateCaptchaCode(6);

            // Create a new CaptchaId
            var CaptchaId = Guid.NewGuid().ToString();

            // Store the code in memory for 10 mins (adjust as needed)
            _cache.Set(CaptchaId, captchaCode, TimeSpan.FromMinutes(10));

            // Generate the image
            var captchaImageBytes = CaptchaService.GenerateCaptchaImage(captchaCode);

            // Convert to Base64 for <img src="data:image/png;base64,..." />
            var base64Image = Convert.ToBase64String(captchaImageBytes);

            // Return JSON: { CaptchaId, CaptchaImage } 
            return Json(new
            {
                CaptchaId = CaptchaId,
                CaptchaImage = $"data:image/png;base64,{base64Image}"
            });
        }

        // GET /captcha/refresh?CaptchaId=<your-guid-here>
        [HttpGet("refresh")]
        public IActionResult RefreshCaptcha(string CaptchaId)
        {
            if (string.IsNullOrEmpty(CaptchaId))
            {
                return BadRequest("CaptchaId is required.");
            }

            // Remove existing captcha code from cache
            _cache.Remove(CaptchaId);

            // Generate a new code
            var newCaptchaCode = CaptchaService.GenerateCaptchaCode(6);

            // Store it in memory
            _cache.Set(CaptchaId, newCaptchaCode, TimeSpan.FromMinutes(10));

            // Generate the new image
            var captchaImageBytes = CaptchaService.GenerateCaptchaImage(newCaptchaCode);
            var base64Image = Convert.ToBase64String(captchaImageBytes);

            // Return JSON
            return Json(new
            {
                CaptchaId = CaptchaId,
                CaptchaImage = $"data:image/png;base64,{base64Image}"
            });
        }
        // POST /captcha/validate
        [HttpPost("validate")]
        public IActionResult ValidateCaptcha([FromBody] CaptchaValidationModel model)
        {
            if (string.IsNullOrEmpty(model.CaptchaId) || string.IsNullOrEmpty(model.CaptchaInput))
            {
                return BadRequest(new { success = false, message = "Captcha ID and input are required." });
            }

            // Retrieve the stored CAPTCHA code from cache
            if (!_cache.TryGetValue(model.CaptchaId, out string storedCaptchaCode))
            {
                return BadRequest(new { success = false, message = "Captcha expired or invalid." });
            }

            // Remove the used CAPTCHA from cache
            _cache.Remove(model.CaptchaId);

            // Validate input
            if (!string.Equals(storedCaptchaCode, model.CaptchaInput, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { success = false, message = "Invalid Captcha. Try again!" });
            }

            return Ok(new { success = true, message = "Captcha validated successfully." });
        }

        // Model for validation
        public class CaptchaValidationModel
        {
            public string CaptchaId { get; set; }
            public string CaptchaInput { get; set; }
        }

    }
}