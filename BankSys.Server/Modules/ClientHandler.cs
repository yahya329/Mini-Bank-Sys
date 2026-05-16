using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BankSys.Server.Modules
{
    internal class ClientHandler
    {
        private readonly Socket _socket;
        private readonly AuthService _auth;

        public ClientHandler(Socket socket, AuthService auth)
        {
            _socket = socket;
            _auth = auth;
        }

        public void Handle()
        {
            string clientId = _socket.RemoteEndPoint.ToString();
            Logger.Log("CONNECT", $"Client connected: {clientId}");

            try
            {
                // ── Step 1: Authentication handshake ──────────────────────
                BankAcc account = Authenticate();
                if (account == null)
                {
                    Send("ERR:Authentication failed. Disconnecting.");
                    return;
                }

                // ── Step 2: Main session loop ─────────────────────────────
                Send("OK:Welcome! Commands: BANKING | CHAT | EXIT");

                while (true)
                {
                    string input = Receive();

                    if (string.IsNullOrEmpty(input) || input == "EXIT")
                    {
                        Logger.Log("SESSION", $"{clientId} disconnected.");
                        break;
                    }

                    switch (input.ToUpper())
                    {
                        case "BANKING":
                            RunBankingSession(account, clientId);
                            break;

                        case "CHAT":
                            RunChatSession(clientId);
                            break;

                        default:
                            Send("ERR:Unknown mode. Type BANKING, CHAT, or EXIT.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("ERROR", $"{clientId} — {ex.Message}");
            }
            finally
            {
                _socket.Close();
            }
        }

        // ── Authentication ───────────────────────────────────────────────
        private BankAcc Authenticate()
        {
            Send("AUTH:Type LOGIN or SIGNUP");
            string choice = Receive()?.ToUpper();

            Send("AUTH:Enter username:");
            string username = Receive();

            Send("AUTH:Enter PIN:");
            string pin = Receive();

            if (choice == "SIGNUP")
            {
                string result = _auth.SignUp(username, pin);
                Send(result);
                if (!result.StartsWith("OK")) return null;

                // auto-login after signup
                var (success, msg, account) = _auth.Login(username, pin);
                Send(msg);
                return success ? account : null;
            }
            else // LOGIN
            {
                var (success, msg, account) = _auth.Login(username, pin);
                Send(msg);
                Logger.Log("AUTH", $"Login attempt for '{username}': {(success ? "SUCCESS" : "FAILED")}");
                return success ? account : null;
            }
        }

        // ── Banking session ──────────────────────────────────────────────
        private void RunBankingSession(BankAcc account, string clientId)
        {
            Send("BANKING:Commands: DEPOSIT:amount | WITHDRAW:amount | BALANCE | DONE");

            while (true)
            {
                string command = Receive();

                if (string.IsNullOrEmpty(command) || command.ToUpper() == "DONE")
                {
                    Send("BANKING:Session ended.");
                    break;
                }

                string response = BankingModule.Handle(command, account);
                Send(response);
                Logger.Log("BANKING", $"{account.CurrUser} — {command} → {response}");
            }
        }

        // ── Chat session ────────────────────────────────────────────────-
        private void RunChatSession(string clientId)
        {
            Send("CHAT:Connected to SecureBank Support. Type 'bye' to exit.");

            while (true)
            {
                string message = Receive();

                if (string.IsNullOrEmpty(message)) break;

                string response = ChatBot.GetResponse(message);
                Send(response);
                Logger.Log("CHAT", $"{clientId} — '{message}' → '{response}'");

                if (response.Contains("DISCONNECT")) break;
            }

            Send("CHAT:Session ended.");
        }

        // ── Helpers ──────────────────────────────────────────────────────
        private void Send(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            _socket.Send(data);
        }

        private string Receive()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = _socket.Receive(buffer);
            return bytesRead == 0 ? null : Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
        }
    }
}
