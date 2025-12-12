using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Data;

namespace Project2Delivery2.DataAccessLayer.Repositories
{
    public interface ITransactionRepository
    {
        List<Transaction> GetAllTransactions();
        Transaction GetTransactionById(long id);
        List<Transaction> GetTransactionsByAccountId(int accountId);
        void AddTransaction(Transaction transaction);
        void UpdateTransaction(Transaction transaction);
        void DeleteTransaction(long id);
    }


}
