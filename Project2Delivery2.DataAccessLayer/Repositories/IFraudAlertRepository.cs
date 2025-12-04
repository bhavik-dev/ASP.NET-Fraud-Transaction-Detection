using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Data;

namespace Project2Delivery2.DataAccessLayer.Repositories
{
    public interface IFraudAlertRepository
    {
        List<FraudAlert> GetAllAlerts();
        FraudAlert GetAlertById(long id);
        List<FraudAlert> GetAlertsByStatus(string status);
        void AddAlert(FraudAlert alert);
        void UpdateAlert(FraudAlert alert);
    }
}
