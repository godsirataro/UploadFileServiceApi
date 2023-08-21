using FileUploadService.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FileUploadservice.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FileController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public FileController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                string baseDirectory = Directory.GetCurrentDirectory();

                string targetDirectory = Path.Combine(baseDirectory, "Uploads");
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                string fileName = Path.GetExtension(file.FileName);
                string filePath = Path.Combine(targetDirectory, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                await _emailService.SendEmailAsync("Upload File", "File uploaded successfully.");
                return Ok("File uploaded successfully.");
            }
            catch (Exception ex)
            {
                await _emailService.SendEmailAsync("Delete File", $"Error deleting file: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading file: " + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] List<string> fileNames)
        {
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();

                foreach (var fileName in fileNames)
                {
                    string filePath = Path.Combine(currentDirectory, "Uploads", fileName);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                await _emailService.SendEmailAsync("Delete File", "File deleting successfully.");
                return Ok("Files deleted successfully.");
            }
            catch (Exception ex)
            {
                await _emailService.SendEmailAsync("Delete File", $"Error deleting file: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting files: " + ex.Message);
            }
        }
    }
}