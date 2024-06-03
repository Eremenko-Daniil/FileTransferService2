using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace FileTransferService2.Pages
{
    public class LogsModel : PageModel
    {
        public string LogContent { get; private set; }

        public void OnGet()
        {
            var logPath = "path_to_your_log_file.log";
            if (System.IO.File.Exists(logPath))
            {
                LogContent = System.IO.File.ReadAllText(logPath);
            }
            else
            {
                LogContent = "Лог-файл не найден.";
            }
        }
    }
}