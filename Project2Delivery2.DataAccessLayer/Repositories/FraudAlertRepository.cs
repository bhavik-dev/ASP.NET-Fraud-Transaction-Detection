using Microsoft.EntityFrameworkCore;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Data;

namespace Project2Delivery2.DataAccessLayer.Repositories
{
    public class FraudAlertRepository : IFraudAlertRepository
    {
        private readonly FraudDetectionDbContext _context;

        public FraudAlertRepository(FraudDetectionDbContext context)
        {
            _context = context;
        }

        public List<FraudAlert> GetAllAlerts()
        {
            return _context.FraudAlerts.ToList();
        }

        public FraudAlert GetAlertById(long id)
        {
            return _context.FraudAlerts.Find(id);
        }

        public List<FraudAlert> GetAlertsByStatus(string status)
        {
            return _context.FraudAlerts
                .Where(a => a.Status == status)
                .ToList();
        }

        public void AddAlert(FraudAlert alert)
        {
            alert.CreatedAt = DateTime.Now;
            _context.FraudAlerts.Add(alert);
            _context.SaveChanges();
        }

        public void UpdateAlert(FraudAlert alert)
        {
            _context.FraudAlerts.Update(alert);
            _context.SaveChanges();
        }
    }
}