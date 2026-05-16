using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BankSys.Client
{
    internal class LiveRatesUDP
    {
        private readonly string _serverIp;
        private readonly int _udpPort;

        public LiveRatesUDP(string serverIp, int udpPort)
        {
            _serverIp = serverIp;
            _udpPort = udpPort;
        }

        public void Run()
        {
            // UDP — no Connect(), no persistent socket
            // Every call is a fresh packet
            Socket udpSocket = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Dgram,
                                          ProtocolType.Udp);

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(_serverIp), _udpPort);

            System.Console.WriteLine("\n[TICKER] Fetching live exchange rates...");

            // Send request — connectionless, no handshake
            byte[] request = Encoding.UTF8.GetBytes("GET_RATES");
            udpSocket.SendTo(request, serverEndPoint);

            // Receive response — server fires back immediately
            byte[] buffer = new byte[1024];
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            int bytesRead = udpSocket.ReceiveFrom(buffer, ref remoteEP);

            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Parse and display: "RATES|USD:48.52|EUR:52.13|GBP:61.47|SAR:12.95"
            DisplayRates(response);

            udpSocket.Close();
        }

        private void DisplayRates(string raw)
        {
            string[] parts = raw.Split('|');

            System.Console.WriteLine("\n╔══════════════════════════╗");
            System.Console.WriteLine("║   SecureBank Live Rates  ║");
            System.Console.WriteLine("╠══════════════════════════╣");

            // Skip first part ("RATES"), display the rest
            for (int i = 1; i < parts.Length; i++)
            {
                string[] kv = parts[i].Split(':');
                if (kv.Length == 2)
                    System.Console.WriteLine($"║  {kv[0]} → {kv[1]} EGP".PadRight(28) + "║");
            }

            System.Console.WriteLine("╚══════════════════════════╝");
        }
    }
}
