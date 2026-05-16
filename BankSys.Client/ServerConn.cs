using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BankSys.Client
{
    internal class ServerConn
    {
        private readonly Socket _socket;

        public ServerConn(Socket socket)
        {
            _socket = socket;
        }

        public void Send(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            _socket.Send(data);
        }

        public string Receive()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = _socket.Receive(buffer);
            return bytesRead == 0 ? null : Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
        }

        public void Close()
        {
            _socket.Close();
        }
    }
}
