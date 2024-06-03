using System;
using System.IO;

namespace FileTransferService2.Pages.Utils
{
    public static class Logger
    {
        private static readonly object lockObj = new object();

        public static void Log(string filePath, string message)
        {
            lock (lockObj)
            {
                using (var writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
        }
    }
}