using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankSys.Server.Modules;

namespace BankSys.Server
{
    internal class Program
    {
        private const int TCP_PORT = 8000;
        private const int UDP_PORT = 8001;

        static void Main(string[] args)
        {
            Logger.Log("SERVER", "SecureBank Server starting...");

            // Shared auth service — one instance for all clients
            AuthService authService = new AuthService();

            // ── Start UDP handler on its own thread ───────────────────────
            LiveRates udpHandler = new LiveRates(UDP_PORT);
            Thread udpThread = new Thread(() => udpHandler.Start());
            udpThread.IsBackground = true;   // dies when main program exits
            udpThread.Start();

            // ── Start TCP listener ────────────────────────────────────────
            Socket tcpListener = new Socket(AddressFamily.InterNetwork,
                                            SocketType.Stream,
                                            ProtocolType.Tcp);

            tcpListener.Bind(new IPEndPoint(IPAddress.Any, TCP_PORT));
            tcpListener.Listen(10);   // queue up to 10 pending connections

            Logger.Log("SERVER", $"TCP listening on port {TCP_PORT}");
            Logger.Log("SERVER", "Waiting for clients...");

            // ── Main accept loop — never blocks on a client ───────────────
            while (true)
            {
                Socket clientSocket = tcpListener.Accept();  // blocks until a client connects

                // Hand off immediately to a new thread
                ClientHandler handler = new ClientHandler(clientSocket, authService);
                Thread clientThread = new Thread(() => handler.Handle());
                clientThread.IsBackground = true;
                clientThread.Start();
                
                Logger.Log("SERVER", $"New client thread started");
            }
        }
    }
}
