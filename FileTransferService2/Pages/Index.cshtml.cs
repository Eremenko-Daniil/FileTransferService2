/*using FileTransferService2.Pages.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;

namespace FileTransferService2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostTransferFilesAsync()
        {
            var sourceConfig = ConfigReader.ReadConfig("Server1.txt");
            var destConfig = ConfigReader.ReadConfig("Server2.txt");

            try
            {
                // Логика передачи файлов
                Logger.Log("transfer.log", $"File transferred successfully from {sourceConfig.Path} to {destConfig.Path}");
            }
            catch (Exception ex)
            {
                Logger.Log("transfer.log", $"Error transferring file: {ex.Message}");
            }

            return RedirectToPage();
        }
        
        public static string CalculateMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public async Task VerifyFileIntegrity(string sourceFilePath, string destFilePath)
        {
            var sourceHash = CalculateMD5(sourceFilePath);
            var destHash = CalculateMD5(destFilePath);

            if (sourceHash == destHash)
            {
                Logger.Log("integrity.log", $"File integrity verified for {sourceFilePath}");
            }
            else
            {
                Logger.Log("integrity.log", $"File integrity check failed for {sourceFilePath}");
            }
        }
    }
}*/

/*using FileTransferService2.Pages.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;

namespace FileTransferService2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostTransferFilesAsync()
        {
            var sourceConfig = ConfigReader.ReadConfig("Server1.txt");
            var destConfig = ConfigReader.ReadConfig("Server2.txt");

            try
            {
                // Логика передачи файлов (временно пропущена)
                // Передача файлов между sourceConfig и destConfig
                // await TransferFilesAsync(sourceConfig, destConfig);

                // Логирование успешной передачи
                Logger.Log("transfer.log", $"File transferred successfully from {sourceConfig.Path} to {destConfig.Path}");

                // Проверка целостности файлов
                await VerifyFileIntegrityAsync(sourceConfig.Path, destConfig.Path);
            }
            catch (Exception ex)
            {
                Logger.Log("transfer.log", $"Error transferring file: {ex.Message}");
            }

            return RedirectToPage();
        }

        public static string CalculateMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public async Task VerifyFileIntegrityAsync(string sourceFilePath, string destFilePath)
        {
            var sourceHash = CalculateMD5(sourceFilePath);
            var destHash = CalculateMD5(destFilePath);

            if (sourceHash == destHash)
            {
                Logger.Log("integrity.log", $"File integrity verified for {sourceFilePath}");
            }
            else
            {
                Logger.Log("integrity.log", $"File integrity check failed for {sourceFilePath}");
            }
        }

        // Пример метода для передачи файлов
        private async Task TransferFilesAsync(ServerConfig sourceConfig, ServerConfig destConfig)
        {
            // Реализуйте логику передачи файлов между серверами
            // Например, используя FTP или другой протокол
        }
    }
}*/

using Microsoft.AspNetCore.Mvc.RazorPages;
using FluentFTP;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace FileTransferService2.Pages
{
    public class IndexModel : PageModel
    {
        public string SourceServerName { get; private set; }
        public string DestServerName { get; private set; }

        public List<string> SourceFiles { get; private set; } = new List<string>();
        public List<string> SourceFolders { get; private set; } = new List<string>();
        public List<string> DestFiles { get; private set; } = new List<string>();
        public List<string> DestFolders { get; private set; } = new List<string>();

        public void OnGet()
        {
            SourceServerName = LoadServerName("Server1.txt");
            DestServerName = LoadServerName("Server2.txt");

            LoadFilesAndFoldersFromServer("Server1.txt", SourceFiles, SourceFolders);
            LoadFilesAndFoldersFromServer("Server2.txt", DestFiles, DestFolders);
        }

        private string LoadServerName(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                var lines = System.IO.File.ReadAllLines(fileName);
                foreach (var line in lines)
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2 && parts[0] == "ИмяСервера")
                    {
                        return parts[1];
                    }
                }
            }
            return "Неизвестный сервер";
        }

        private void LoadFilesAndFoldersFromServer(string fileName, List<string> fileList, List<string> folderList)
        {
            if (System.IO.File.Exists(fileName))
            {
                var config = System.IO.File.ReadAllLines(fileName);
                string host = "";
                int port = 21;
                string username = "anonymous";
                string password = "anonymous@domain.com"; // Используйте стандартные учетные данные для анонимного подключения

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

                try
                {
                    ftpClient.Connect();

                    foreach (var item in ftpClient.GetListing())
                    {
                        if (item.Type == FtpObjectType.File)
                        {
                            fileList.Add(item.FullName);
                        }
                        else if (item.Type == FtpObjectType.Directory)
                        {
                            folderList.Add(item.FullName);
                        }
                    }

                    ftpClient.Disconnect();
                }
                catch (Exception ex)
                {
                    // Логирование ошибки для отладки
                    System.Diagnostics.Debug.WriteLine($"Ошибка подключения к серверу {host}: {ex.Message}");
                }
            }
        }

        public string GetFileIcon(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".pdf" => "/img/pdf-icon.png",
                ".doc" => "/img/doc-icon.png",
                ".docx" => "/img/doc-icon.png",
                ".xls" => "/img/xls-icon.png",
                ".xlsx" => "/img/xls-icon.png",
                ".txt" => "/img/txt-icon.png",
                _ => "/img/file-icon.png",
            };
        }
    }
}