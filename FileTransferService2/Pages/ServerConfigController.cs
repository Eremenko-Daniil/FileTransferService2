using Microsoft.AspNetCore.Mvc;
using FluentFTP;
using System.IO;
using System.Net;

namespace FileTransferService2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerConfigController : ControllerBase
    {
        [HttpGet("CheckConnection")]
        public IActionResult CheckConnection(int server)
        {
            string message;

            try
            {
                string fileName = server == 1 ? "Server1.txt" : "Server2.txt";
                if (!System.IO.File.Exists(fileName))
                {
                    return new JsonResult(new { message = $"Файл конфигурации {fileName} не найден." });
                }

                var config = System.IO.File.ReadAllLines(fileName);
                string host = "";
                int port = 29;
                string username = "anonymous"; // замените на свои учетные данные
                string password = "anonymous"; // замените на свои учетные данные

                foreach (var line in config)
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        switch (parts[0])
                        {
                            case "IPPort":
                                var ipPortParts = parts[1].Split(':');
                                host = ipPortParts[0];
                                port = int.Parse(ipPortParts[1]);
                                break;
                        }
                    }
                }

                var ftpClient = new FtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password)
                };

                ftpClient.Config.ConnectTimeout = 5000; // Устанавливаем таймаут через параметры конфигурации

                ftpClient.Connect();

                message = $"Подключение к серверу {server} установлено!";
            }
            catch (Exception ex)
            {
                message = $"Не удалось подключиться к серверу {server}. Ошибка: {ex.Message}";
            }

            return new JsonResult(new { message });
        }

        [HttpDelete("DeleteConfigFile")]
        public IActionResult DeleteConfigFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
                return new JsonResult(new { success = true });
            }
            return new JsonResult(new { success = false });
        }
    }
}