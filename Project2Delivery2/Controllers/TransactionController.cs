using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Repositories;
using Project2Delivery2.ViewModels;

namespace Project2Delivery2.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFraudAlertRepository _fraudAlertRepository;
        private readonly IMerchantRepository _merchantRepository;

        public TransactionController(
            ITransactionRepository transactionRepository,
            IFraudAlertRepository fraudAlertRepository,
            IMerchantRepository merchantRepository)
        {
            _transactionRepository = transactionRepository;
            _fraudAlertRepository = fraudAlertRepository;
            _merchantRepository = merchantRepository;
        }

        public IActionResult Index()
        {
            var transactions = _transactionRepository.GetAllTransactions();

            ViewData["PageTitle"] = "All Transactions";
            ViewData["TotalCount"] = transactions.Count;
            ViewData["FlaggedCount"] = transactions.Count(t => t.Status == "Flagged");
            ViewData["TotalAmount"] = transactions.Sum(t => t.Amount);

            return View(transactions);
        }

        public IActionResult Details(long id)
        {
            var transaction = _transactionRepository.GetTransactionById(id);

            if (transaction == null)
            {
                return NotFound();
            }

            ViewBag.Title = "Transaction Details";
            ViewBag.TransactionId = id;

            if (transaction.Amount > 1000 || transaction.Merchant.MerchantRiskScore > 0.7m)
            {
                ViewBag.HighRisk = true;
                ViewBag.AlertMessage = "This transaction has been flagged as high risk!";
                ViewBag.AlertClass = "alert-danger";
            }
            else
            {
                ViewBag.HighRisk = false;
                ViewBag.AlertMessage = "This transaction appears normal.";
                ViewBag.AlertClass = "alert-success";
            }

            var alerts = _fraudAlertRepository.GetAllAlerts().Where(a => a.TransactionId == id).ToList();
            ViewBag.AlertCount = alerts.Count;

            return View(transaction);
        }

        [Authorize]
        public IActionResult Create()
        {
            // Get data for dropdowns
            var transactions = _transactionRepository.GetAllTransactions();
            var accounts = transactions.Select(t => t.Account).DistinctBy(a => a.AccountId).ToList();
            var merchants = _merchantRepository.GetAllMerchants();
            var devices = transactions.Select(t => t.Device).DistinctBy(d => d.DeviceId).ToList();

            // Create SelectLists
            ViewBag.Accounts = new SelectList(accounts, "AccountId", "HolderName");
            ViewBag.Merchants = new SelectList(merchants, "MerchantId", "Name");
            ViewBag.Devices = new SelectList(devices, "DeviceId", "DeviceHash");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Analyst")]
        public IActionResult Create(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.CreatedAt = DateTime.Now;
                transaction.Timestamp = DateTime.Now;
                transaction.Account = null;
                transaction.Merchant = null;
                transaction.Device = null;

                _transactionRepository.AddTransaction(transaction);

                if (transaction.Amount > 1000)
                {
                    var alert = new FraudAlert
                    {
                        TransactionId = transaction.TransactionId,
                        AlertLevel = "High",
                        Status = "Open",
                        CreatedAt = DateTime.Now
                    };
                    _fraudAlertRepository.AddAlert(alert);

                    TempData["SuccessMessage"] = "Transaction created and flagged for review due to high amount.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Transaction created successfully.";
                }

                return RedirectToAction("Index");
            }

            var transactions = _transactionRepository.GetAllTransactions();
            var accounts = transactions.Select(t => t.Account).DistinctBy(a => a.AccountId).ToList();
            var merchants = _merchantRepository.GetAllMerchants();
            var devices = transactions.Select(t => t.Device).DistinctBy(d => d.DeviceId).ToList();

            ViewBag.Accounts = new SelectList(accounts, "AccountId", "HolderName");
            ViewBag.Merchants = new SelectList(merchants, "MerchantId", "Name");
            ViewBag.Devices = new SelectList(devices, "DeviceId", "DeviceHash");

            return View(transaction);
        }

        public IActionResult ByAccount(int accountId)
        {
            var transactions = _transactionRepository.GetTransactionsByAccountId(accountId);
            ViewBag.AccountId = accountId;
            return View(transactions);
        }

        public IActionResult DetailedView(long id)
        {
            var transaction = _transactionRepository.GetTransactionById(id);

            if (transaction == null)
            {
                return NotFound();
            }

            var relatedAlerts = _fraudAlertRepository.GetAllAlerts()
                .Where(a => a.TransactionId == id).ToList();

            var accountTransactions = _transactionRepository
                .GetTransactionsByAccountId(transaction.AccountId);

            var riskFactors = new System.Collections.Generic.List<string>();
            decimal riskScore = 0m;

            if (transaction.Amount > 1000)
            {
                riskFactors.Add("High transaction amount (>" + 1000.ToString("C") + ")");
                riskScore += 0.3m;
            }

            if (transaction.Merchant.MerchantRiskScore > 0.7m)
            {
                riskFactors.Add("High-risk merchant (score: " + transaction.Merchant.MerchantRiskScore.ToString("P") + ")");
                riskScore += 0.4m;
            }

            if (accountTransactions.Count == 1)
            {
                riskFactors.Add("First transaction on account");
                riskScore += 0.2m;
            }

            if (transaction.Device.FirstSeen > DateTime.Now.AddDays(-7))
            {
                riskFactors.Add("New device (first seen less than 7 days ago)");
                riskScore += 0.15m;
            }

            string riskLevel = riskScore > 0.7m ? "High" : riskScore > 0.4m ? "Medium" : "Low";

            var viewModel = new TransactionDetailViewModel
            {
                Transaction = transaction,
                RelatedAlerts = relatedAlerts,
                RiskScore = riskScore,
                RiskLevel = riskLevel,
                RiskFactors = riskFactors,
                AccountTransactionCount = accountTransactions.Count,
                AccountTotalSpent = accountTransactions.Sum(t => t.Amount),
                IsFirstTransaction = accountTransactions.Count == 1,
                IsNewDevice = transaction.Device.FirstSeen > DateTime.Now.AddDays(-7),
                DeviceTransactionCount = _transactionRepository.GetAllTransactions()
                    .Count(t => t.DeviceId == transaction.DeviceId)
            };

            return View(viewModel);
        }
    }
}