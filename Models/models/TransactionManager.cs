using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class TransactionManager
    {
        private List<Transaction> transactions = new List<Transaction>();
        private int nextId = 1;

        public Transaction CreateTransaction(Cashier cashier)
        {
            var transaction = new Transaction
            {
                TransactionId = nextId++,
                CashierId = cashier.UserId,
                Cashier = cashier
            };
            return transaction;
        }

        public bool CompleteTransaction(Transaction transaction, decimal amountPaid)
        {
            if (transaction.CompleteSale(amountPaid))
            {
                transactions.Add(transaction);
                transaction.Cashier?.UpdateSalesMetrics(transaction.TotalAmount);
                return true;
            }
            return false;
        }

        public List<Transaction> GetDailySales(DateTime date)
        {
            return transactions.Where(t =>
                t.TransactionDate.Date == date.Date &&
                t.Status == "Completed"
            ).ToList();
        }

        public decimal GetTotalSales(DateTime date)
        {
            return GetDailySales(date).Sum(t => t.TotalAmount);
        }

        public List<Transaction> GetAllTransactions()
        {
            return transactions;
        }
    }
}
