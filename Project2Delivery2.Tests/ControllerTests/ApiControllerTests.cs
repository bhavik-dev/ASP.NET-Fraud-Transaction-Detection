using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project2Delivery2.Controllers;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Project2Delivery2.Tests.ControllerTests
{
    [TestClass]
    [TestCategory("API")]
    public class ApiControllerTests
    {
        private Mock<ITransactionRepository> _mockTransactionRepo;
        private Mock<IFraudAlertRepository> _mockFraudAlertRepo;
        private ApiController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockFraudAlertRepo = new Mock<IFraudAlertRepository>();
            _controller = new ApiController(_mockTransactionRepo.Object, _mockFraudAlertRepo.Object);
        }

        [TestMethod]
        [Description("Verify GetTransactions returns OkResult with data")]
        [TestCategory("CI")]
        public void GetTransactions_ReturnsOkResultWithTransactions()
        {
            // Arrange
            var mockTransactions = new List<Transaction>
            {
                new Transaction { TransactionId = 1, TransactionRef = "TXN001" }
            };
            _mockTransactionRepo.Setup(r => r.GetAllTransactions()).Returns(mockTransactions);

            // Act
            var result = _controller.GetTransactions() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(result.Value, typeof(List<Transaction>));
        }

        [TestMethod]
        [Description("Verify GetTransaction with valid ID returns OkResult")]
        public void GetTransaction_ValidId_ReturnsOkResult()
        {
            // Arrange
            var transaction = new Transaction { TransactionId = 5, TransactionRef = "TXN005" };
            _mockTransactionRepo.Setup(r => r.GetTransactionById(5)).Returns(transaction);

            // Act
            var result = _controller.GetTransaction(5) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        [Description("Verify GetTransaction with invalid ID returns NotFound")]
        public void GetTransaction_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockTransactionRepo.Setup(r => r.GetTransactionById(999)).Returns((Transaction)null);

            // Act
            var result = _controller.GetTransaction(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        [Description("Verify SearchTransactions with empty status returns BadRequest")]
        public void SearchTransactions_EmptyStatus_ReturnsBadRequest()
        {
            // Act
            var result = _controller.SearchTransactions("");

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        [Description("Verify GetStatistics returns correct statistics")]
        public void GetStatistics_ReturnsCorrectStats()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction { Amount = 100m, Status = "Completed" },
                new Transaction { Amount = 200m, Status = "Flagged" }
            };
            var alerts = new List<FraudAlert>
            {
                new FraudAlert { AlertLevel = "High" }
            };
            _mockTransactionRepo.Setup(r => r.GetAllTransactions()).Returns(transactions);
            _mockFraudAlertRepo.Setup(r => r.GetAllAlerts()).Returns(alerts);

            // Act
            var result = _controller.GetStatistics();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JsonResult));
        }

        [TestMethod]
        [Description("Verify DeleteTransaction returns NoContent on success")]
        public void DeleteTransaction_ValidId_ReturnsNoContent()
        {
            // Arrange
            var transaction = new Transaction { TransactionId = 5 };
            _mockTransactionRepo.Setup(r => r.GetTransactionById(5)).Returns(transaction);

            // Act
            var result = _controller.DeleteTransaction(5);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        [Description("Verify DeleteTransaction with invalid ID returns NotFound")]
        public void DeleteTransaction_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockTransactionRepo.Setup(r => r.GetTransactionById(999)).Returns((Transaction)null);

            // Act
            var result = _controller.DeleteTransaction(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _controller = null;
        }
    }
}
