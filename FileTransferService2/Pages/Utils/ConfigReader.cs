using System.IO;

namespace FileTransferService2.Pages.Utils
{
    public class ServerConfig
    {
        public string ServerName { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
    }

    public static class ConfigReader
    {
        public static ServerConfig ReadConfig(string filePath)
        {
            var config = new ServerConfig();
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    switch (parts[0].Trim())
                    {
                        case "ИмяСервера":
                            config.ServerName = parts[1].Trim();
                            break;
                        case "IP":
                            config.IP = parts[1].Trim();
                            break;
                        case "Порт":
                            config.Port = int.Parse(parts[1].Trim());
                            break;
                        case "Путь":
                            config.Path = parts[1].Trim();
                            break;
                    }
                }
            }
            return config;
        }
    }
}