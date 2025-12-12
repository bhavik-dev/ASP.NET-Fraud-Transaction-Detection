using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Data;

namespace Project2Delivery2.DataAccessLayer.Repositories
{
    public interface IMerchantRepository
    {
        List<Merchant> GetAllMerchants();
        Merchant GetMerchantById(int id);
        List<Merchant> GetHighRiskMerchants();
    }
}
