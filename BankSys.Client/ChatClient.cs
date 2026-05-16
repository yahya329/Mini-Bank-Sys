using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSys.Client
{
    internal class ChatClient
    {
        private readonly ServerConn _conn;

        public ChatClient(ServerConn conn)
        {
            _conn = conn;
        }

        public void Run()
        {
            // Tell server we want chat mode
            _conn.Send("CHAT");

            // Server greeting
            string greeting = _conn.Receive();
            System.Console.WriteLine($"\n[SUPPORT] {greeting}");

            while (true)
            {
                System.Console.Write("\n[YOU]> ");
                string message = System.Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(message)) continue;

                _conn.Send(message);
                string response = _conn.Receive();
                System.Console.WriteLine($"[SUPPORT] {response}");

                // Server signals disconnect (user typed 'bye')
                if (response.Contains("Session ended")) break;
            }
        }
    }
}
