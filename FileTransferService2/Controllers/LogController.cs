using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly string logFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

        [HttpGet("GetLogs")]
        public async Task<IActionResult> GetLogs()
        {
            try
            {
                EnsureLogFolderExists();

                var logFiles = Directory.GetFiles(logFolderPath, "*.txt");
                var logContent = new StringBuilder();
                foreach (var logFile in logFiles)
                {
                    logContent.AppendLine(await System.IO.File.ReadAllTextAsync(logFile));
                }
                return Ok(new { logs = logContent.ToString() });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("ClearLogs")]
        public IActionResult ClearLogs()
        {
            try
            {
                EnsureLogFolderExists();

                var logFiles = Directory.GetFiles(logFolderPath, "*.txt");
                foreach (var logFile in logFiles)
                {
                    System.IO.File.Delete(logFile);
                }
                return Ok(new { message = "Logs cleared successfully." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private void EnsureLogFolderExists()
        {
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            var logFilePath = Path.Combine(logFolderPath, $"log-{System.DateTime.Now.ToString("yyyyMMdd")}.txt");
            if (!System.IO.File.Exists(logFilePath))
            {
                System.IO.File.Create(logFilePath).Dispose();
            }
        }
    }
}