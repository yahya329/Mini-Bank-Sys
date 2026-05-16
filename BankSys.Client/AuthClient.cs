using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSys.Client
{
    internal class AuthClient
    {
        private readonly ServerConn _conn;

        public AuthClient(ServerConn conn)
        {
            _conn = conn;
        }

        // Returns true if auth succeeded
        public bool Run()
        {
            // Server speaks first — it tells us what to do
            string prompt = _conn.Receive();
            System.Console.WriteLine($"[SERVER] {prompt}");

            System.Console.Write("Your choice (LOGIN / SIGNUP): ");
            string choice = System.Console.ReadLine()?.ToUpper();
            _conn.Send(choice);

            // Username
            string userPrompt = _conn.Receive();
            System.Console.WriteLine($"[SERVER] {userPrompt}");
            System.Console.Write("> ");
            string username = System.Console.ReadLine();
            _conn.Send(username);

            // PIN
            string pinPrompt = _conn.Receive();
            System.Console.WriteLine($"[SERVER] {pinPrompt}");
            System.Console.Write("> ");
            string pin = System.Console.ReadLine();
            _conn.Send(pin);

            // Result
            string result = _conn.Receive();
            System.Console.WriteLine($"[SERVER] {result}");

            // If signup, server sends a second message (auto-login result)
            if (choice == "SIGNUP")
            {
                string autoLogin = _conn.Receive();
                System.Console.WriteLine($"[SERVER] {autoLogin}");
                return autoLogin.StartsWith("OK");
            }

            return result.StartsWith("OK");
        }
    }
}
