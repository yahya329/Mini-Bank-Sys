using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSys.Server.Modules
{
    internal static class ChatBot
    {
        public static string GetResponse(string message)
        {
            string msg = message.ToLower();

            if (msg.Contains("help"))
                return "BOT:How can I assist you? You can ask about hours, loans, or account issues.";

            if (msg.Contains("hours") || msg.Contains("open"))
                return "BOT:We are open Sunday to Thursday, 9 AM to 5 PM.";

            if (msg.Contains("loan"))
                return "BOT:We offer personal and home loans. Visit any branch or call 19XXX for details.";

            if (msg.Contains("balance"))
                return "BOT:Please use the Banking module to check your balance securely.";

            if (msg.Contains("hello") || msg.Contains("hi"))
                return "BOT:Hello! Welcome to SecureBank support. How can I help you today?";

            if (msg.Contains("bye") || msg.Contains("exit"))
                return "BOT:DISCONNECT";  // signal to close session

            return "BOT:I didn't understand that. Try asking about 'hours', 'loan', or type 'help'.";
        }
    }
}
