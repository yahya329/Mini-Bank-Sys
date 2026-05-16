using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSys.Client
{
    internal class BankingClient
    {
        private readonly ServerConn _conn;

        public BankingClient(ServerConn conn)
        {
            _conn = conn;
        }

        public void Run()
        {
            // Tell server we want banking mode
            _conn.Send("BANKING");

            // Server responds with instructions
            string instructions = _conn.Receive();
            System.Console.WriteLine($"\n[SERVER] {instructions}");

            while (true)
            {
                System.Console.Write("\n[BANKING]> ");
                string input = System.Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input)) continue;

                _conn.Send(input);
                string response = _conn.Receive();
                System.Console.WriteLine($"[SERVER] {response}");

                // Server signals end of banking session
                if (response.Contains("Session ended")) break;
            }
        }
    }
}
