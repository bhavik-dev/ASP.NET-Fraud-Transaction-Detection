using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Data;

namespace Project2Delivery2.DataAccessLayer.Repositories
{
    public class MerchantRepository : IMerchantRepository
    {
        private readonly FraudDetectionDbContext _context;

        public MerchantRepository(FraudDetectionDbContext context)
        {
            _context = context;
        }

        public List<Merchant> GetAllMerchants()
        {
            return _context.Merchants.ToList();
        }

        public Merchant GetMerchantById(int id)
        {
            return _context.Merchants.Find(id);
        }

        public List<Merchant> GetHighRiskMerchants()
        {
            return _context.Merchants
                .Where(m => m.MerchantRiskScore > 0.6m)
                .ToList();
        }
    }
}