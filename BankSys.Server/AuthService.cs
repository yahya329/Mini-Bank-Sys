using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSys.Server
{
    internal class AuthService
    {
        private readonly Dictionary<string, (string pin, BankAcc accounts)>
            _users = new Dictionary<string, (string, BankAcc)>();

        private readonly object _lock = new object();

        public string SignUp(string username, string pin)
        {
            lock (_lock)
            {
                if (_users.ContainsKey(username))
                    return "ERR:This user already Exist !";

                _users[username] = (pin, new BankAcc(username, 0));

                return "OK:account created successfully :)";

            }

        }

        public (bool done, string msg, BankAcc acc) Login(string username, string pin)
        {
            lock (_lock)
            {
                if (!_users.ContainsKey(username))
                {
                    return (false, "User not found :(", null);
                }

                var user = _users[username];

                if (user.pin != pin)
                {
                    return (false, "Pin Not Correct !", null);
                }

                return (true, "OK:Login successful.", _users[username].accounts);

            }
        }

    }
}

