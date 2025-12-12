using Microsoft.EntityFrameworkCore;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Data;

namespace Project2Delivery2.DataAccessLayer.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly FraudDetectionDbContext _context;

        public TransactionRepository(FraudDetectionDbContext context)
        {
            _context = context;
        }

        public List<Transaction> GetAllTransactions()
        {
            _context.ChangeTracker.Clear();

            return _context.Transactions
                .AsNoTracking() // Don't track entities
                .Include(t => t.Account)
                .Include(t => t.Merchant)
                .Include(t => t.Device)
                .OrderByDescending(t => t.TransactionId) // Show newest first
                .ToList();
        }

        public Transaction GetTransactionById(long id)
        {
            return _context.Transactions
                .AsNoTracking() // Add this
                .Include(t => t.Account)
                .Include(t => t.Merchant)
                .Include(t => t.Device)
                .FirstOrDefault(t => t.TransactionId == id);
        }

        public List<Transaction> GetTransactionsByAccountId(int accountId)
        {
            return _context.Transactions
                .AsNoTracking() // Add this
                .Include(t => t.Account)
                .Include(t => t.Merchant)
                .Include(t => t.Device)
                .Where(t => t.AccountId == accountId)
                .ToList();
        }

        public void AddTransaction(Transaction transaction)
        {
            transaction.Account = null;
            transaction.Merchant = null;
            transaction.Device = null;

            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        public void UpdateTransaction(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            _context.SaveChanges();
        }

        public void DeleteTransaction(long id)
        {
            var transaction = _context.Transactions.Find(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                _context.SaveChanges();
            }
        }
    }
}