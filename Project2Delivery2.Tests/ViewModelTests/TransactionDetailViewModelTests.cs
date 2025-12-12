using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.ViewModels;
using System.Collections.Generic;

namespace Project2Delivery2.Tests.ViewModelTests
{
    [TestClass]
    [TestCategory("ViewModel")]
    public class TransactionDetailViewModelTests
    {
        [TestMethod]
        [Description("Verify ViewModel properties are set correctly")]
        public void TransactionDetailViewModel_PropertiesSetCorrectly()
        {
            // Arrange & Act
            var viewModel = new TransactionDetailViewModel
            {
                Transaction = new Transaction { TransactionId = 1 },
                RiskScore = 0.75m,
                RiskLevel = "High",
                RiskFactors = new List<string> { "High amount", "New device" },
                AccountTransactionCount = 5,
                AccountTotalSpent = 5000m,
                IsFirstTransaction = false,
                IsNewDevice = true
            };

            // Assert
            Assert.AreEqual(0.75m, viewModel.RiskScore);
            Assert.AreEqual("High", viewModel.RiskLevel);
            Assert.AreEqual(2, viewModel.RiskFactors.Count);
            Assert.AreEqual(5, viewModel.AccountTransactionCount);
            Assert.AreEqual(5000m, viewModel.AccountTotalSpent);
            Assert.IsFalse(viewModel.IsFirstTransaction);
            Assert.IsTrue(viewModel.IsNewDevice);
        }

        [TestMethod]
        [Description("Verify ViewModel can hold related alerts")]
        public void TransactionDetailViewModel_CanHoldRelatedAlerts()
        {
            // Arrange
            var alerts = new List<FraudAlert>
            {
                new FraudAlert { AlertId = 1, AlertLevel = "High" },
                new FraudAlert { AlertId = 2, AlertLevel = "Medium" }
            };

            // Act
            var viewModel = new TransactionDetailViewModel
            {
                RelatedAlerts = alerts
            };

            // Assert
            Assert.AreEqual(2, viewModel.RelatedAlerts.Count);
            Assert.AreEqual("High", viewModel.RelatedAlerts[0].AlertLevel);
        }
    }
}