using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BankSys.Server.Modules
{
    internal class LiveRates
    {
        private readonly int _port;

        public LiveRates(int port)
        {
            _port = port;
        }

        public void Start()
        {
            Socket udpSocket = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Dgram,
                                          ProtocolType.Udp);

            udpSocket.Bind(new IPEndPoint(IPAddress.Any, _port));
            Logger.Log("UDP", $"Live Rates listener started on port {_port}");

            byte[] buffer = new byte[1024];

            while (true)
            {
                EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                int bytesRead = udpSocket.ReceiveFrom(buffer, ref clientEndPoint);
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                Logger.Log("UDP", $"Request from {clientEndPoint}: {request}");

                if (request == "GET_RATES")
                {
                    string rates = GetLiveRates();
                    byte[] response = Encoding.UTF8.GetBytes(rates);
                    udpSocket.SendTo(response, clientEndPoint);
                }
            }
        }

        private string GetLiveRates()
        {
            // Simulated — in real life this hits an API
            Random rng = new Random();
            decimal usd = 48.5m + (decimal)(rng.NextDouble() * 0.5);
            decimal eur = 52.1m + (decimal)(rng.NextDouble() * 0.5);
            decimal gbp = 61.3m + (decimal)(rng.NextDouble() * 0.5);
            decimal sar = 12.9m + (decimal)(rng.NextDouble() * 0.2);

            return $"RATES|USD:{usd:F2}|EUR:{eur:F2}|GBP:{gbp:F2}|SAR:{sar:F2}";
        }
    }
}
