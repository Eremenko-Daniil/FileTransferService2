using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;

namespace FileTransferService2.Pages
{
    public class ServerConfigModel : PageModel
    {
        [BindProperty]
        public string SourceServerName { get; set; }
        [BindProperty]
        public string SourceServerPath { get; set; }
        [BindProperty]
        public string SourceServerIPPort { get; set; }
        [BindProperty]
        public string DestServerName { get; set; }
        [BindProperty]
        public string DestServerPath { get; set; }
        [BindProperty]
        public string DestServerIPPort { get; set; }

        public List<string> ConfigFiles { get; private set; }

        public void OnGet()
        {
            LoadConfigFiles();
        }

        public IActionResult OnPost()
        {
            // Сохранение конфигурации серверов
            SaveConfig("Server1.txt", SourceServerName, SourceServerPath, SourceServerIPPort);
            SaveConfig("Server2.txt", DestServerName, DestServerPath, DestServerIPPort);

            LoadConfigFiles();
            return RedirectToPage();
        }

        private void LoadConfigFiles()
        {
            ConfigFiles = new List<string>
            {
                "Server1.txt",
                "Server2.txt"
            };
        }

        private void SaveConfig(string fileName, string serverName, string serverPath, string serverIPPort)
        {
            var config = new[]
            {
                $"ИмяСервера={serverName}",
                $"Путь={serverPath}",
                $"IPPort={serverIPPort}"
            };
            System.IO.File.WriteAllLines(fileName, config);
        }

        private void LoadConfigFromFile(int server)
        {
            string fileName = server == 1 ? "Server1.txt" : "Server2.txt";
            if (System.IO.File.Exists(fileName))
            {
                var lines = System.IO.File.ReadAllLines(fileName);
                foreach (var line in lines)
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        switch (parts[0])
                        {
                            case "ИмяСервера":
                                if (server == 1)
                                    SourceServerName = parts[1];
                                else
                                    DestServerName = parts[1];
                                break;
                            case "Путь":
                                if (server == 1)
                                    SourceServerPath = parts[1];
                                else
                                    DestServerPath = parts[1];
                                break;
                            case "IPPort":
                                if (server == 1)
                                    SourceServerIPPort = parts[1];
                                else
                                    DestServerIPPort = parts[1];
                                break;
                        }
                    }
                }
            }
        }
    }
}