using Microsoft.AspNetCore.Mvc;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Repositories;
using System.Linq;

namespace Project2Delivery2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFraudAlertRepository _fraudAlertRepository;

        public ApiController(ITransactionRepository transactionRepository, IFraudAlertRepository fraudAlertRepository)
        {
            _transactionRepository = transactionRepository;
            _fraudAlertRepository = fraudAlertRepository;
        }

        [HttpGet("transactions")]
        public IActionResult GetTransactions()
        {
            var transactions = _transactionRepository.GetAllTransactions();
            return Ok(transactions);
        }

        [HttpGet("transactions/{id}")]
        public IActionResult GetTransaction([FromRoute] long id)
        {
            var transaction = _transactionRepository.GetTransactionById(id);

            if (transaction == null)
            {
                return NotFound(new { message = $"Transaction {id} not found" });
            }

            return Ok(transaction);
        }

        [HttpGet("transactions/search")]
        public IActionResult SearchTransactions([FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Status parameter is required");
            }

            var transactions = _transactionRepository.GetAllTransactions()
                .Where(t => t.Status.Equals(status, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!transactions.Any())
            {
                return NotFound($"No transactions found with status: {status}");
            }

            return Ok(transactions);
        }

        [HttpGet("stats")]
        public JsonResult GetStatistics()
        {
            var transactions = _transactionRepository.GetAllTransactions();
            var alerts = _fraudAlertRepository.GetAllAlerts();

            var stats = new
            {
                totalTransactions = transactions.Count,
                totalAmount = transactions.Sum(t => t.Amount),
                averageAmount = transactions.Average(t => t.Amount),
                flaggedCount = transactions.Count(t => t.Status == "Flagged"),
                alertCount = alerts.Count,
                highRiskAlerts = alerts.Count(a => a.AlertLevel == "High")
            };

            return new JsonResult(stats);
        }

        [HttpGet("transactions/{id}/summary")]
        public IActionResult GetTransactionSummary([FromRoute] long id)
        {
            var transaction = _transactionRepository.GetTransactionById(id);

            if (transaction == null)
            {
                return NotFound();
            }

            var summary = $"Transaction {transaction.TransactionRef}: ${transaction.Amount} - {transaction.Status}";
            return Content(summary, "text/plain");
        }

        [HttpGet("transactions/{id}/xml")]
        public ContentResult GetTransactionXml([FromRoute] long id)
        {
            var transaction = _transactionRepository.GetTransactionById(id);

            if (transaction == null)
            {
                return Content("<error>Transaction not found</error>", "application/xml");
            }

            var xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<transaction>
    <id>{transaction.TransactionId}</id>
    <reference>{transaction.TransactionRef}</reference>
    <amount currency=""{transaction.Currency}"">{transaction.Amount}</amount>
    <status>{transaction.Status}</status>
</transaction>";

            return Content(xml, "application/xml");
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return StatusCode(200, new { status = "healthy", timestamp = System.DateTime.Now });
        }

        [HttpPost("transactions")]
        public IActionResult CreateTransaction([FromBody] Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _transactionRepository.AddTransaction(transaction);

            return CreatedAtAction(
                nameof(GetTransaction),
                new { id = transaction.TransactionId },
                transaction
            );
        }

        [HttpGet("redirect")]
        public IActionResult Redirect()
        {
            return RedirectToAction(nameof(GetTransactions));
        }

        [HttpGet("secure")]
        public IActionResult SecureEndpoint([FromHeader(Name = "Authorization")] string authorization)
        {
            if (string.IsNullOrEmpty(authorization))
            {
                return Unauthorized(new { message = "Authorization header required" });
            }

            return Ok(new { message = "Access granted" });
        }

        [HttpDelete("transactions/{id}")]
        public IActionResult DeleteTransaction([FromRoute] long id)
        {
            var transaction = _transactionRepository.GetTransactionById(id);

            if (transaction == null)
            {
                return NotFound();
            }

            _transactionRepository.DeleteTransaction(id);

            return NoContent();
        }

        [HttpGet("alerts")]
        public IActionResult GetAlerts([FromQuery] string level = null)
        {
            var alerts = _fraudAlertRepository.GetAllAlerts();

            if (!string.IsNullOrEmpty(level))
            {
                alerts = alerts.Where(a => a.AlertLevel.Equals(level, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return Ok(new
            {
                count = alerts.Count,
                data = alerts
            });
        }
    }
}