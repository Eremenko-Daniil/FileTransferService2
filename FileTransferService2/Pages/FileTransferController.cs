/*using Microsoft.AspNetCore.Mvc;
using FluentFTP;
using System;
using System.IO;
using System.Net;

namespace FileTransferService2.Pages
{
    [Route("FileTransfer")]
    public class FileTransferController : Controller
    {
        [HttpPost("MoveFile")]
        public IActionResult MoveFile([FromBody] MoveFileRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.Target))
            {
                return BadRequest(new { success = false, message = "Invalid request parameters" });
            }

            string sourceServerConfig = request.Target == "source" ? "Server2.txt" : "Server1.txt";
            string destinationServerConfig = request.Target == "source" ? "Server1.txt" : "Server2.txt";

            try
            {
                // Скачивание файла с исходного сервера
                string tempFilePath = DownloadFileFromServer(sourceServerConfig, request.FileName);

                // Загрузка файла на целевой сервер
                UploadFileToServer(destinationServerConfig, tempFilePath, request.FileName);

                // Удаление временного файла
                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DeleteFile")]
        public IActionResult DeleteFile([FromBody] FileOperationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.Server))
            {
                return BadRequest(new { success = false, message = "Invalid request parameters" });
            }

            string serverConfigFile = request.Server == "source" ? "Server1.txt" : "Server2.txt";

            try
            {
                DeleteFileFromServer(serverConfigFile, request.FileName);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("RenameFile")]
        public IActionResult RenameFile([FromBody] RenameFileRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OldFileName) || string.IsNullOrEmpty(request.NewFileName) || string.IsNullOrEmpty(request.Server))
            {
                return BadRequest(new { success = false, message = "Invalid request parameters" });
            }

            string serverConfigFile = request.Server == "source" ? "Server1.txt" : "Server2.txt";

            try
            {
                RenameFileOnServer(serverConfigFile, request.OldFileName, request.NewFileName);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private string DownloadFileFromServer(string serverConfigFile, string remoteFilePath)
        {
            var (host, port, username, password) = LoadFtpServerConfig(serverConfigFile);

            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(remoteFilePath));

            using var ftpClient = new FtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password)
            };

            ftpClient.Connect();
            ftpClient.DownloadFile(tempFilePath, remoteFilePath);
            ftpClient.Disconnect();

            return tempFilePath;
        }

        private void UploadFileToServer(string serverConfigFile, string localFilePath, string remoteFilePath)
        {
            var (host, port, username, password) = LoadFtpServerConfig(serverConfigFile);

            using var ftpClient = new FtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password)
            };

            ftpClient.Connect();
            ftpClient.UploadFile(localFilePath, remoteFilePath);
            ftpClient.Disconnect();
        }

        private void DeleteFileFromServer(string serverConfigFile, string remoteFilePath)
        {
            var (host, port, username, password) = LoadFtpServerConfig(serverConfigFile);

            using var ftpClient = new FtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password)
            };

            ftpClient.Connect();
            ftpClient.DeleteFile(remoteFilePath);
            ftpClient.Disconnect();
        }

        private void RenameFileOnServer(string serverConfigFile, string oldFilePath, string newFilePath)
        {
            var (host, port, username, password) = LoadFtpServerConfig(serverConfigFile);

            using var ftpClient = new FtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password)
            };

            ftpClient.Connect();
            ftpClient.Rename(oldFilePath, newFilePath);
            ftpClient.Disconnect();
        }

        private (string host, int port, string username, string password) LoadFtpServerConfig(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                throw new FileNotFoundException($"Configuration file '{fileName}' not found.");
            }

            var lines = System.IO.File.ReadAllLines(fileName);
            string host = "", username = "anonymous", password = "anonymous@domain.com";
            int port = 21;

            foreach (var line in lines)
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
                        case "ИмяПользователя":
                            username = parts[1];
                            break;
                        case "Пароль":
                            password = parts[1];
                            break;
                    }
                }
            }

            return (host, port, username, password);
        }
    }

    public class MoveFileRequest
    {
        public string FileName { get; set; }
        public string Target { get; set; }
    }

    public class FileOperationRequest
    {
        public string FileName { get; set; }
        public string Server { get; set; }
    }

    public class RenameFileRequest
    {
        public string OldFileName { get; set; }
        public string NewFileName { get; set; }
        public string Server { get; set; }
    }
}
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentFTP;
using System;
using System.IO;
using System.Net;

namespace FileTransferService2.Pages
{
    [Route("FileTransfer")]
    public class FileTransferController : Controller
    {
        private readonly ILogger<FileTransferController> _logger;
        public FileTransferController(ILogger<FileTransferController> logger)
        {
            _logger = logger;
        }

        [HttpPost("MoveFile")]
        public IActionResult MoveFile([FromBody] MoveFileRequest request)
        {
            _logger.LogInformation($"Attempting to move file: {request.FileName} to {request.Target}");

            if (request == null || string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.Target))
            {
                return BadRequest(new { success = false, message = "Invalid request parameters" });
            }

            string sourceConfigFile = request.Target == "destination" ? "Server1.txt" : "Server2.txt";
            string destConfigFile = request.Target == "destination" ? "Server2.txt" : "Server1.txt";

            try
            {
                string tempFilePath = Path.GetTempFileName();

                // Скачивание файла с исходного сервера
                DownloadFileFromServer(sourceConfigFile, request.FileName, tempFilePath);

                // Загрузка файла на целевой сервер
                UploadFileToServer(destConfigFile, tempFilePath, request.FileName);

                // Удаление временного файла
                System.IO.File.Delete(tempFilePath);
                                
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }

            _logger.LogInformation($"File {request.FileName} moved successfully to {request.Target}");
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files, string server)
        {
            if (files == null || files.Count == 0 || string.IsNullOrEmpty(server))
            {
                return BadRequest(new { success = false, message = "Invalid request parameters" });
            }

            string serverConfigFile = server == "source" ? "Server1.txt" : "Server2.txt";

            foreach (var file in files)
            {
                string tempFilePath = Path.GetTempFileName();
                try
                {
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var (host, port, username, password) = LoadFtpServerConfig(serverConfigFile);
                    string remoteFilePath = Path.Combine("/", file.FileName); // Можете изменить путь назначения, если нужно

                    using var ftpClient = new FtpClient(host)
                    {
                        Port = port,
                        Credentials = new NetworkCredential(username, password)
                    };

                    ftpClient.Connect();
                    ftpClient.UploadFile(tempFilePath, remoteFilePath);
                    ftpClient.Disconnect();

                    // Удаление временного файла
                    System.IO.File.Delete(tempFilePath);
                }
                catch (Exception ex)
                {
                    // Удаление временного файла в случае ошибки
                    System.IO.File.Delete(tempFilePath);
                    return StatusCode(500, new { success = false, message = ex.Message });
                }
            }

            return Ok(new { success = true });
        }


        [HttpPost("DeleteFile")]
        public IActionResult DeleteFile([FromBody] FileOperationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.Server))
            {
                return BadRequest(new { success = false, message = "Invalid request parameters" });
            }

            string serverConfigFile = request.Server == "source" ? "Server1.txt" : "Server2.txt";

            try
            {
                var (host, port, username, password) = LoadFtpServerConfig(serverConfigFile);

                using var ftpClient = new FtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password)
                };

                ftpClient.Connect();
                ftpClient.DeleteFile(request.FileName);
                ftpClient.Disconnect();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("RenameFile")]
        public IActionResult RenameFile([FromBody] RenameFileRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OldFileName) || string.IsNullOrEmpty(request.NewFileName) || string.IsNullOrEmpty(request.Server))
            {
                return BadRequest(new { success = false, message = "Invalid request parameters" });
            }

            string serverConfigFile = request.Server == "source" ? "Server1.txt" : "Server2.txt";

            try
            {
                var (host, port, username, password) = LoadFtpServerConfig(serverConfigFile);

                using var ftpClient = new FtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password)
                };

                ftpClient.Connect();
                ftpClient.Rename(request.OldFileName, request.NewFileName);
                ftpClient.Disconnect();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private void DownloadFileFromServer(string serverConfigFile, string remoteFilePath, string localFilePath)
        {
            var (host, port, username, password) = LoadFtpServerConfig(serverConfigFile);

            using var ftpClient = new FtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password)
            };

            ftpClient.Connect();
            ftpClient.DownloadFile(localFilePath, remoteFilePath);
            ftpClient.Disconnect();
        }

        private void UploadFileToServer(string serverConfigFile, string localFilePath, string remoteFilePath)
        {
            var (host, port, username, password) = LoadFtpServerConfig(serverConfigFile);

            using var ftpClient = new FtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password)
            };

            ftpClient.Connect();
            ftpClient.UploadFile(localFilePath, remoteFilePath);
            ftpClient.Disconnect();
        }

        private (string host, int port, string username, string password) LoadFtpServerConfig(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                throw new FileNotFoundException($"Configuration file '{fileName}' not found.");
            }

            var lines = System.IO.File.ReadAllLines(fileName);
            string host = "", username = "anonymous", password = "anonymous@domain.com";
            int port = 21;

            foreach (var line in lines)
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
                        case "ИмяПользователя":
                            username = parts[1];
                            break;
                        case "Пароль":
                            password = parts[1];
                            break;
                    }
                }
            }

            return (host, port, username, password);
        }
    }

    public class MoveFileRequest
    {
        public string FileName { get; set; }
        public string Target { get; set; }
    }

    public class FileOperationRequest
    {
        public string FileName { get; set; }
        public string Server { get; set; }
    }

    public class RenameFileRequest
    {
        public string OldFileName { get; set; }
        public string NewFileName { get; set; }
        public string Server { get; set; }
    }
}