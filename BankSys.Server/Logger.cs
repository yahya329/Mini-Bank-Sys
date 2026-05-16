using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BankSys.Server
{
    public static class Logger
    {
        private static readonly string _logPath = "server_log.txt";
        private static readonly object _lock = new object();

        public static void Log(string category, string message)
        {
            string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{category}] {message}";
            Console.WriteLine(line);

            lock (_lock)
            {
                File.AppendAllText(_logPath, line + Environment.NewLine);
            }
        }
    }
}
