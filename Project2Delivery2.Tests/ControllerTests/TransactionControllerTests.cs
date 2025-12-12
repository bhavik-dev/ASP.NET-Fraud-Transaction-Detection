using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project2Delivery2.Controllers;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Repositories;
using System.Collections.Generic;

namespace Project2Delivery2.Tests.ControllerTests
{
    [TestClass]
    [TestCategory("Controller")]
    [TestProperty("Layer", "Presentation")]
    public class TransactionControllerTests
    {
        private Mock<ITransactionRepository> _mockTransactionRepo;
        private Mock<IFraudAlertRepository> _mockFraudAlertRepo;
        private Mock<IMerchantRepository> _mockMerchantRepo;
        private TransactionController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockFraudAlertRepo = new Mock<IFraudAlertRepository>();
            _mockMerchantRepo = new Mock<IMerchantRepository>();

            _controller = new TransactionController(
                _mockTransactionRepo.Object,
                _mockFraudAlertRepo.Object,
                _mockMerchantRepo.Object
            );
        }

        [TestMethod]
        [Description("Verify Index action returns ViewResult")]
        [TestCategory("UnitTest")]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var mockTransactions = new List<Transaction>
            {
                new Transaction { TransactionId = 1, Amount = 100m, Account = new Account(), Merchant = new Merchant(), Device = new Device() }
            };
            _mockTransactionRepo.Setup(r => r.GetAllTransactions()).Returns(mockTransactions);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        [Description("Verify Index action passes correct model to view")]
        public void Index_PassesTransactionListToView()
        {
            // Arrange
            var mockTransactions = new List<Transaction>
            {
                new Transaction { TransactionId = 1, Account = new Account(), Merchant = new Merchant(), Device = new Device() },
                new Transaction { TransactionId = 2, Account = new Account(), Merchant = new Merchant(), Device = new Device() }
            };
            _mockTransactionRepo.Setup(r => r.GetAllTransactions()).Returns(mockTransactions);

            // Act
            var result = _controller.Index() as ViewResult;
            var model = result.Model as List<Transaction>;

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
        }

        [TestMethod]
        [Description("Verify Index action sets ViewData correctly")]
        public void Index_SetsViewDataCorrectly()
        {
            // Arrange
            var mockTransactions = new List<Transaction>
            {
                new Transaction { TransactionId = 1, Amount = 100m, Status = "Completed", Account = new Account(), Merchant = new Merchant(), Device = new Device() },
                new Transaction { TransactionId = 2, Amount = 200m, Status = "Flagged", Account = new Account(), Merchant = new Merchant(), Device = new Device() }
            };
            _mockTransactionRepo.Setup(r => r.GetAllTransactions()).Returns(mockTransactions);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("All Transactions", result.ViewData["PageTitle"]);
            Assert.AreEqual(2, result.ViewData["TotalCount"]);
            Assert.AreEqual(1, result.ViewData["FlaggedCount"]);
            Assert.AreEqual(300m, result.ViewData["TotalAmount"]);
        }

        [TestMethod]
        [Description("Verify Details action with valid ID returns ViewResult")]
        public void Details_ValidId_ReturnsViewResult()
        {
            // Arrange
            var transaction = new Transaction
            {
                TransactionId = 5,
                Amount = 500m,
                Account = new Account { HolderName = "John" },
                Merchant = new Merchant { Name = "Amazon", MerchantRiskScore = 0.2m },
                Device = new Device()
            };
            _mockTransactionRepo.Setup(r => r.GetTransactionById(5)).Returns(transaction);
            _mockFraudAlertRepo.Setup(r => r.GetAllAlerts()).Returns(new List<FraudAlert>());

            // Act
            var result = _controller.Details(5);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        [Description("Verify Details action with invalid ID returns NotFound")]
        public void Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockTransactionRepo.Setup(r => r.GetTransactionById(999)).Returns((Transaction)null);

            // Act
            var result = _controller.Details(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        [Description("Verify Details sets ViewBag properties correctly")]
        public void Details_SetsViewBagCorrectly()
        {
            // Arrange
            var transaction = new Transaction
            {
                TransactionId = 5,
                Amount = 1500m, // High amount
                Account = new Account(),
                Merchant = new Merchant { MerchantRiskScore = 0.8m }, // High risk
                Device = new Device()
            };
            _mockTransactionRepo.Setup(r => r.GetTransactionById(5)).Returns(transaction);
            _mockFraudAlertRepo.Setup(r => r.GetAllAlerts()).Returns(new List<FraudAlert>());

            // Act
            var result = _controller.Details(5) as ViewResult;

            // Assert
            Assert.AreEqual("Transaction Details", result.ViewData["Title"]);
            Assert.AreEqual(5L, result.ViewData["TransactionId"]);
            Assert.AreEqual(true, result.ViewData["HighRisk"]);
            Assert.AreEqual("alert-danger", result.ViewData["AlertClass"]);
        }

        [TestMethod]
        [Description("Verify Create GET action returns ViewResult")]
        public void Create_Get_ReturnsViewResult()
        {
            // Arrange
            _mockTransactionRepo.Setup(r => r.GetAllTransactions()).Returns(new List<Transaction>());
            _mockMerchantRepo.Setup(r => r.GetAllMerchants()).Returns(new List<Merchant>());

            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

       

        [TestMethod]
        [Description("Verify ByAccount action returns correct view")]
        public void ByAccount_ValidAccountId_ReturnsViewWithTransactions()
        {
            // Arrange
            var accountTransactions = new List<Transaction>
            {
                new Transaction { AccountId = 1, Account = new Account(), Merchant = new Merchant(), Device = new Device() }
            };
            _mockTransactionRepo.Setup(r => r.GetTransactionsByAccountId(1)).Returns(accountTransactions);

            // Act
            var result = _controller.ByAccount(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.ViewData["AccountId"]);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _controller?.Dispose();
        }
    }
}