using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSys.Server
{
    public class BankAcc
    {
        private decimal _balance = 0;
        private readonly object _lock = new object();  // thread safety

        public string CurrUser { get; }

        public BankAcc(string owner, decimal initialBalance = 0)
        {
            CurrUser = owner;
            _balance = initialBalance;
        }

        public string Deposit(decimal amount)
        {
            if (amount <= 0)
                return "ERR:Amount must be greater than zero.";

            lock (_lock)
            {
                _balance += amount;
                return $"OK:Deposited {amount:F2} EGP. New balance: {_balance:F2} EGP.";
            }
        }

        public string Withdraw(decimal amount)
        {
            if (amount <= 0)
                return "ERR:Amount must be greater than zero.";

            lock (_lock)
            {
                if (amount > _balance)
                    return $"ERR:Insufficient funds. Balance is {_balance:F2} EGP.";

                _balance -= amount;
                return $"OK:Withdrew {amount:F2} EGP. New balance: {_balance:F2} EGP.";
            }
        }

        public string GetBalance()
        {
            lock (_lock)
            {
                return $"OK:Current balance: {_balance:F2} EGP.";
            }
        }
    }
}
