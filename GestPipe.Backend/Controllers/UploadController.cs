using GestPipe.Backend.Models.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GestPipe.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Upload avatar image
        /// POST /api/upload/avatar
        /// </summary>
        [HttpPost("avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file uploaded." });
            }

            var file = request.File;

            try
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!Array.Exists(allowedExtensions, ext => ext == extension))
                {
                    return BadRequest(new { success = false, message = "Only JPG, JPEG, PNG files are allowed." });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, message = "File size must not exceed 5MB." });
                }

                // Create avatars folder if not exists
                var uploadsFolder = Path.Combine(_env.WebRootPath, "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    _logger.LogInformation("Created avatars directory: {Path}", uploadsFolder);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate public URL
                var url = $"{Request.Scheme}://{Request.Host}/avatars/{fileName}";

                _logger.LogInformation("Avatar uploaded successfully: {Url}", url);

                return Ok(new { success = true, url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar");
                return StatusCode(500, new { success = false, message = "Error uploading file." });
            }
        }
    }
}