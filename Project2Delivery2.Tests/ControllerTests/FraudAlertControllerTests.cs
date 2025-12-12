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
    public class FraudAlertControllerTests
    {
        private Mock<IFraudAlertRepository> _mockFraudAlertRepo;
        private Mock<ITransactionRepository> _mockTransactionRepo;
        private FraudAlertController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockFraudAlertRepo = new Mock<IFraudAlertRepository>();
            _mockTransactionRepo = new Mock<ITransactionRepository>();
            _controller = new FraudAlertController(_mockFraudAlertRepo.Object, _mockTransactionRepo.Object);
        }

        [TestMethod]
        [Description("Verify Index returns ViewResult with alerts")]
        public void Index_ReturnsViewResultWithAlerts()
        {
            // Arrange
            var alerts = new List<FraudAlert>
            {
                new FraudAlert { AlertId = 1, AlertLevel = "High" }
            };
            _mockFraudAlertRepo.Setup(r => r.GetAllAlerts()).Returns(alerts);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<FraudAlert>));
        }

        [TestMethod]
        [Description("Verify Details with valid ID returns ViewResult")]
        public void Details_ValidId_ReturnsViewResult()
        {
            // Arrange
            var alert = new FraudAlert { AlertId = 1, TransactionId = 2 };
            var transaction = new Transaction { TransactionId = 2 };
            _mockFraudAlertRepo.Setup(r => r.GetAlertById(1)).Returns(alert);
            _mockTransactionRepo.Setup(r => r.GetTransactionById(2)).Returns(transaction);

            // Act
            var result = _controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        [Description("Verify Details with invalid ID returns NotFound")]
        public void Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockFraudAlertRepo.Setup(r => r.GetAlertById(999)).Returns((FraudAlert)null);

            // Act
            var result = _controller.Details(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        [Description("Verify Review POST updates alert and redirects")]
        public void Review_Post_UpdatesAlertAndRedirects()
        {
            // Arrange
            var alert = new FraudAlert { AlertId = 1, Status = "Open" };
            _mockFraudAlertRepo.Setup(r => r.GetAlertById(1)).Returns(alert);

            // Act
            var result = _controller.Review(1, "Resolved", 2) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            _mockFraudAlertRepo.Verify(r => r.UpdateAlert(It.IsAny<FraudAlert>()), Times.Once);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _controller = null;
        }
    }
}