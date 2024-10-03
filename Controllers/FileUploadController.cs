using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

public class FileUploadController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public FileUploadController(IWebHostEnvironment env, IConfiguration config)
    {
        _env = env;
        _config = config;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            // Get the upload directory from appsettings.json
            string externalFolderPath = _config["AppSettings:UploadDirectory"];

            // Ensure the directory exists
            if (!Directory.Exists(externalFolderPath))
            {
                Directory.CreateDirectory(externalFolderPath);
            }

            // Create the full file path
            string filePath = Path.Combine(externalFolderPath, file.FileName);

            // Save the file to the external folder
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { filePath });
        }

        return BadRequest("File upload failed.");
    }
}
