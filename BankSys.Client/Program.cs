using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankSys.Client
{
    internal class Program
    {
        private const string SERVER_IP = "127.0.0.1"; // localhost for testing
        private const int TCP_PORT = 8000;
        private const int UDP_PORT = 8001;

        static void Main(string[] args)
        {
            System.Console.WriteLine("╔════════════════════════════╗");
            System.Console.WriteLine("║   SecureBank Client v1.0   ║");
            System.Console.WriteLine("╚════════════════════════════╝");

            // ── Open ONE TCP connection for the whole session ─────────────
            Socket tcpSocket = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream,
                                          ProtocolType.Tcp);

            try
            {
                tcpSocket.Connect(new IPEndPoint(IPAddress.Parse(SERVER_IP), TCP_PORT));
                System.Console.WriteLine("[INFO] Connected to SecureBank server.\n");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[ERROR] Could not connect to server: {ex.Message}");
                System.Console.ReadKey();
                return;
            }

            ServerConn conn = new ServerConn(tcpSocket);

            // ── Authentication ────────────────────────────────────────────
            AuthClient auth = new AuthClient(conn);
            bool loggedIn = auth.Run();

            if (!loggedIn)
            {
                System.Console.WriteLine("[ERROR] Authentication failed. Exiting.");
                conn.Close();
                System.Console.ReadKey();
                return;
            }

            // ── Main menu loop ────────────────────────────────────────────
            BankingClient banking = new BankingClient(conn);
            ChatClient chat = new ChatClient(conn);
            LiveRatesUDP ticker = new LiveRatesUDP(SERVER_IP, UDP_PORT);

            while (true)
            {
                System.Console.WriteLine("\n╔══════════════════════════╗");
                System.Console.WriteLine("║     SecureBank Menu      ║");
                System.Console.WriteLine("╠══════════════════════════╣");
                System.Console.WriteLine("║  [1] Banking             ║");
                System.Console.WriteLine("║  [2] Support Chat        ║");
                System.Console.WriteLine("║  [3] Live Rates          ║");
                System.Console.WriteLine("║  [4] Logout              ║");
                System.Console.WriteLine("╚══════════════════════════╝");
                System.Console.Write("Choice: ");

                string choice = System.Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        banking.Run();
                        break;

                    case "2":
                        chat.Run();
                        break;

                    case "3":
                        ticker.Run();
                        break;

                    case "4":
                        conn.Send("EXIT");
                        System.Console.WriteLine("[INFO] Logged out. Goodbye!");
                        conn.Close();
                        System.Console.ReadKey();
                        return;

                    default:
                        System.Console.WriteLine("[INFO] Invalid choice. Try 1, 2, 3, or 4.");
                        break;
                }
            }
        }
    }
}
