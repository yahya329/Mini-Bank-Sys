using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSys.Server.Modules
{
    public static class BankingModule
    {
        // command format: "DEPOSIT:100" or "WITHDRAW:50" or "BALANCE"
        public static string Handle(string command, BankAcc account)
        {
            if (command == "BALANCE")
                return account.GetBalance();

            string[] parts = command.Split(':');

            if (parts.Length != 2)
                return "ERR:Unknown command. Use DEPOSIT:amount, WITHDRAW:amount, or BALANCE.";

            string action = parts[0].ToUpper();
            string rawAmount = parts[1];

            if (!decimal.TryParse(rawAmount, out decimal amount))
                return "ERR:Invalid amount.";

            switch (action)
            {
                case "DEPOSIT": return account.Deposit(amount);
                case "WITHDRAW": return account.Withdraw(amount);
                default: return "ERR:Unknown command.";
            }
        }
    }
}
