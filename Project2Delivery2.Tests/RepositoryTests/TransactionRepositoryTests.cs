using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project2Delivery2.Tests.RepositoryTests
    {
        [TestClass]
        [TestCategory("Repository")]
        [TestProperty("Layer", "DataAccess")]
        public class TransactionRepositoryTests
        {
            private Mock<ITransactionRepository> _mockRepository;

            [TestInitialize]
            public void Setup()
            {
                // Arrange - Create mock repository before each test
                _mockRepository = new Mock<ITransactionRepository>();
            }

            [TestMethod]
            [Description("Verify GetAllTransactions returns list of transactions")]
            [TestCategory("UnitTest")]
            public void GetAllTransactions_ReturnsListOfTransactions()
            {
                // Arrange
                var mockTransactions = new List<Transaction>
            {
                new Transaction { TransactionId = 1, TransactionRef = "TXN001", Amount = 100.50m },
                new Transaction { TransactionId = 2, TransactionRef = "TXN002", Amount = 200.75m }
            };
                _mockRepository.Setup(r => r.GetAllTransactions()).Returns(mockTransactions);

                // Act
                var result = _mockRepository.Object.GetAllTransactions();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("TXN001", result[0].TransactionRef);
            }

            [TestMethod]
            [Description("Verify GetTransactionById returns correct transaction")]
            public void GetTransactionById_ValidId_ReturnsTransaction()
            {
                // Arrange
                var expectedTransaction = new Transaction
                {
                    TransactionId = 5,
                    TransactionRef = "TXN005",
                    Amount = 500.00m,
                    Status = "Completed"
                };
                _mockRepository.Setup(r => r.GetTransactionById(5)).Returns(expectedTransaction);

                // Act
                var result = _mockRepository.Object.GetTransactionById(5);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(5, result.TransactionId);
                Assert.AreEqual("TXN005", result.TransactionRef);
                Assert.AreEqual(500.00m, result.Amount);
                Assert.AreEqual("Completed", result.Status);
            }

            [TestMethod]
            [Description("Verify GetTransactionById with invalid ID returns null")]
            public void GetTransactionById_InvalidId_ReturnsNull()
            {
                // Arrange
                _mockRepository.Setup(r => r.GetTransactionById(999)).Returns((Transaction)null);

                // Act
                var result = _mockRepository.Object.GetTransactionById(999);

                // Assert
                Assert.IsNull(result);
            }

            [TestMethod]
            [Description("Verify AddTransaction adds transaction successfully")]
            public void AddTransaction_ValidTransaction_AddsSuccessfully()
            {
                // Arrange
                var newTransaction = new Transaction
                {
                    TransactionRef = "TXN999",
                    AccountId = 1,
                    MerchantId = 1,
                    DeviceId = 1,
                    Amount = 999.99m,
                    Currency = "USD",
                    Status = "Pending"
                };

                // Act
                _mockRepository.Object.AddTransaction(newTransaction);

                // Assert
                _mockRepository.Verify(r => r.AddTransaction(It.IsAny<Transaction>()), Times.Once);
            }

            [TestMethod]
            [Description("Verify GetTransactionsByAccountId filters correctly")]
            public void GetTransactionsByAccountId_ValidAccountId_ReturnsFilteredList()
            {
                // Arrange
                var accountTransactions = new List<Transaction>
            {
                new Transaction { TransactionId = 1, AccountId = 1, Amount = 100m },
                new Transaction { TransactionId = 3, AccountId = 1, Amount = 200m }
            };
                _mockRepository.Setup(r => r.GetTransactionsByAccountId(1)).Returns(accountTransactions);

                // Act
                var result = _mockRepository.Object.GetTransactionsByAccountId(1);

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.IsTrue(result.All(t => t.AccountId == 1));
            }

            [TestMethod]
            [Description("Verify DeleteTransaction removes transaction")]
            public void DeleteTransaction_ValidId_DeletesSuccessfully()
            {
                // Arrange
                long transactionId = 5;

                // Act
                _mockRepository.Object.DeleteTransaction(transactionId);

                // Assert
                _mockRepository.Verify(r => r.DeleteTransaction(5), Times.Once);
            }

            [TestCleanup]
            public void Cleanup()
            {
                // Cleanup after each test
                _mockRepository = null;
            }
        }
    }
