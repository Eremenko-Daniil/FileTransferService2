using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace FileTransferService2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManager1Controller : ControllerBase
    {
        [HttpPost("FileOperations")]
        public IActionResult FileOperations([FromBody] object operation)
        {
            // Обработка операций с файлами для сервера 1
            return Ok(new { message = "Operation successful on Server1" });
        }

        [HttpGet("GetImage")]
        public IActionResult GetImage(string path)
        {
            // Возврат изображения для сервера 1
            var imagePath = Path.Combine("wwwroot", path);
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound();
            }

            var image = System.IO.File.OpenRead(imagePath);
            return File(image, "image/jpeg");
        }
    }
}