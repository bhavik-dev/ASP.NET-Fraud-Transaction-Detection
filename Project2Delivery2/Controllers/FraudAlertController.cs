using Microsoft.AspNetCore.Mvc;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Repositories;
using System.Linq;

namespace Project2Delivery2.Controllers
{
    public class FraudAlertController : Controller
    {
        private readonly IFraudAlertRepository _fraudAlertRepository;
        private readonly ITransactionRepository _transactionRepository;

        public FraudAlertController(IFraudAlertRepository fraudAlertRepository, ITransactionRepository transactionRepository)
        {
            _fraudAlertRepository = fraudAlertRepository;
            _transactionRepository = transactionRepository;
        }

        // GET: FraudAlert
        public IActionResult Index()
        {
            var alerts = _fraudAlertRepository.GetAllAlerts();
            return View(alerts);
        }

        // GET: FraudAlert/Details/5
        public IActionResult Details(long id)
        {
            var alert = _fraudAlertRepository.GetAlertById(id);
            if (alert == null)
            {
                return NotFound();
            }

            alert.Transaction = _transactionRepository.GetTransactionById(alert.TransactionId);
            return View(alert);
        }

        // GET: FraudAlert/Review/5
        public IActionResult Review(long id)
        {
            var alert = _fraudAlertRepository.GetAlertById(id);
            if (alert == null)
            {
                return NotFound();
            }

            alert.Transaction = _transactionRepository.GetTransactionById(alert.TransactionId);
            return View(alert);
        }

        // POST: FraudAlert/Review/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Review(long id, string status, int? assignedTo)
        {
            var alert = _fraudAlertRepository.GetAlertById(id);

            if (alert != null)
            {
                alert.Status = status;
                alert.AssignedTo = assignedTo;
                _fraudAlertRepository.UpdateAlert(alert);

                TempData["SuccessMessage"] = "Alert #" + id + " has been updated to '" + status + "' status.";
                TempData["AlertLevel"] = alert.AlertLevel;
                TempData["UpdatedCount"] = 1;
            }
            else
            {
                TempData["ErrorMessage"] = "Alert not found.";
            }

            return RedirectToAction("Index");
        }
    }
}