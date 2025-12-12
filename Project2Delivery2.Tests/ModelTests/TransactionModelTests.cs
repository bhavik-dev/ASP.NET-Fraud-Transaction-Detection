using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project2Delivery2.DataAccessLayer.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Project2Delivery2.Tests.ModelTests
{
    [TestClass]
    [TestCategory("Model")]
    public class TransactionModelTests
    {
        [TestMethod]
        [Description("Verify Transaction model validates required fields")]
        [TestCategory("Validation")]
        public void Transaction_RequiredFields_ValidationFails()
        {
            // Arrange
            var transaction = new Transaction
            {
                // Missing required fields
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(transaction);
            var isValid = Validator.TryValidateObject(transaction, context, validationResults, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(validationResults.Count > 0);
        }

        [TestMethod]
        [Description("Verify Transaction with valid data passes validation")]
        public void Transaction_ValidData_ValidationPasses()
        {
            // Arrange
            var transaction = new Transaction
            {
                TransactionRef = "TXN001",
                AccountId = 1,
                MerchantId = 1,
                DeviceId = 1,
                Amount = 100.50m,
                Currency = "USD",
                Status = "Completed"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(transaction);
            var isValid = Validator.TryValidateObject(transaction, context, validationResults, true);

            // Assert
            Assert.IsTrue(isValid);
            Assert.AreEqual(0, validationResults.Count);
        }

        [TestMethod]
        [Description("Verify Currency must be exactly 3 characters")]
        public void Transaction_InvalidCurrencyLength_ValidationFails()
        {
            // Arrange
            var transaction = new Transaction
            {
                TransactionRef = "TXN001",
                AccountId = 1,
                MerchantId = 1,
                DeviceId = 1,
                Amount = 100m,
                Currency = "US", // Invalid - too short
                Status = "Completed"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(transaction);
            var isValid = Validator.TryValidateObject(transaction, context, validationResults, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Currency")));
        }

        [TestMethod]
        [Description("Verify TransactionRef length validation")]
        public void Transaction_TransactionRefTooShort_ValidationFails()
        {
            // Arrange
            var transaction = new Transaction
            {
                TransactionRef = "TXN", // Too short (< 5 chars)
                AccountId = 1,
                MerchantId = 1,
                DeviceId = 1,
                Amount = 100m,
                Currency = "USD",
                Status = "Completed"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(transaction);
            var isValid = Validator.TryValidateObject(transaction, context, validationResults, true);

            // Assert
            Assert.IsFalse(isValid);
        }
    }
}