using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project2Delivery2.DataAccessLayer.Models;

namespace Project2Delivery2.DataAccessLayer.Data
{
    public class FraudDetectionDbContext : IdentityDbContext<ApplicationUser>
    {
        public FraudDetectionDbContext(DbContextOptions<FraudDetectionDbContext> options)
            : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<FraudAlert> FraudAlerts { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ModelVersion> ModelVersions { get; set; }
        public DbSet<ModelScore> ModelScores { get; set; }
        public DbSet<TransactionFeature> TransactionFeatures { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Relationships

            // Transaction -> Account
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaction -> Merchant
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Merchant)
                .WithMany(m => m.Transactions)
                .HasForeignKey(t => t.MerchantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaction -> Device
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Device)
                .WithMany(d => d.Transactions)
                .HasForeignKey(t => t.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            // FraudAlert -> Transaction
            modelBuilder.Entity<FraudAlert>()
                .HasOne(fa => fa.Transaction)
                .WithMany(t => t.FraudAlerts)
                .HasForeignKey(fa => fa.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            // FraudAlert -> User (AssignedTo)
            modelBuilder.Entity<FraudAlert>()
                .HasOne(fa => fa.AssignedUser)
                .WithMany(u => u.AssignedAlerts)
                .HasForeignKey(fa => fa.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull);

            // ModelScore -> Transaction
            modelBuilder.Entity<ModelScore>()
                .HasOne<Transaction>()
                .WithMany(t => t.ModelScores)
                .HasForeignKey(ms => ms.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ModelScore -> ModelVersion
            modelBuilder.Entity<ModelScore>()
                .HasOne<ModelVersion>()
                .WithMany()
                .HasForeignKey(ms => ms.ModelVersionId)
                .OnDelete(DeleteBehavior.Restrict);

            // TransactionFeature -> Transaction
            modelBuilder.Entity<TransactionFeature>()
                .HasOne<Transaction>()
                .WithMany(t => t.TransactionFeatures)
                .HasForeignKey(tf => tf.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Feedback -> User (CreatedBy)
            modelBuilder.Entity<Feedback>()
                .HasOne<User>()
                .WithMany(u => u.CreatedFeedbacks)
                .HasForeignKey(f => f.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Username = "admin", Email = "admin@fraud.com", PasswordHash = "hashed_password_123", Role = "Admin", CreatedAt = new DateTime(2024, 1, 1) },
                new User { UserId = 2, Username = "analyst1", Email = "analyst1@fraud.com", PasswordHash = "hashed_password_456", Role = "Analyst", CreatedAt = new DateTime(2024, 2, 1) },
                new User { UserId = 3, Username = "analyst2", Email = "analyst2@fraud.com", PasswordHash = "hashed_password_789", Role = "Analyst", CreatedAt = new DateTime(2024, 3, 1) }
            );

            // Seed Accounts
            modelBuilder.Entity<Account>().HasData(
                new Account { AccountId = 1, AccountNumber = "ACC001", HolderName = "John Doe", CreatedAt = new DateTime(2024, 5, 1) },
                new Account { AccountId = 2, AccountNumber = "ACC002", HolderName = "Jane Smith", CreatedAt = new DateTime(2024, 8, 1) },
                new Account { AccountId = 3, AccountNumber = "ACC003", HolderName = "Bob Wilson", CreatedAt = new DateTime(2024, 11, 10) },
                new Account { AccountId = 4, AccountNumber = "ACC004", HolderName = "Alice Johnson", CreatedAt = new DateTime(2024, 6, 15) }
            );

            // Seed Merchants
            modelBuilder.Entity<Merchant>().HasData(
                new Merchant { MerchantId = 1, Name = "Amazon", Category = "E-commerce", MerchantRiskScore = 0.2m, CreatedAt = new DateTime(2019, 1, 1) },
                new Merchant { MerchantId = 2, Name = "Unknown Merchant", Category = "Gambling", MerchantRiskScore = 0.85m, CreatedAt = new DateTime(2024, 10, 1) },
                new Merchant { MerchantId = 3, Name = "Walmart", Category = "Retail", MerchantRiskScore = 0.15m, CreatedAt = new DateTime(2014, 1, 1) },
                new Merchant { MerchantId = 4, Name = "Crypto Exchange", Category = "Cryptocurrency", MerchantRiskScore = 0.70m, CreatedAt = new DateTime(2024, 9, 1) },
                new Merchant { MerchantId = 5, Name = "Target", Category = "Retail", MerchantRiskScore = 0.18m, CreatedAt = new DateTime(2015, 6, 1) }
            );

            // Seed Devices
            modelBuilder.Entity<Device>().HasData(
                new Device { DeviceId = 1, DeviceHash = "DEV001ABC", LastIp = "192.168.1.1", LastGeo = "New York, US", FirstSeen = new DateTime(2024, 5, 1) },
                new Device { DeviceId = 2, DeviceHash = "DEV002XYZ", LastIp = "10.0.0.1", LastGeo = "Unknown", FirstSeen = new DateTime(2024, 11, 19) },
                new Device { DeviceId = 3, DeviceHash = "DEV003QWE", LastIp = "203.45.67.89", LastGeo = "Singapore", FirstSeen = new DateTime(2024, 11, 10) },
                new Device { DeviceId = 4, DeviceHash = "DEV004RTY", LastIp = "172.16.0.1", LastGeo = "London, UK", FirstSeen = new DateTime(2024, 7, 15) }
            );

            // Seed Transactions
            modelBuilder.Entity<Transaction>().HasData(
                new Transaction
                {
                    TransactionId = 1,
                    TransactionRef = "TXN001",
                    AccountId = 1,
                    MerchantId = 1,
                    DeviceId = 1,
                    Amount = 150.50m,
                    Currency = "USD",
                    Timestamp = new DateTime(2024, 11, 20, 10, 0, 0),
                    Status = "Completed",
                    CreatedAt = new DateTime(2024, 11, 20, 10, 0, 0)
                },
                new Transaction
                {
                    TransactionId = 2,
                    TransactionRef = "TXN002",
                    AccountId = 2,
                    MerchantId = 2,
                    DeviceId = 2,
                    Amount = 5000.00m,
                    Currency = "USD",
                    Timestamp = new DateTime(2024, 11, 20, 11, 30, 0),
                    Status = "Flagged",
                    CreatedAt = new DateTime(2024, 11, 20, 11, 30, 0)
                },
                new Transaction
                {
                    TransactionId = 3,
                    TransactionRef = "TXN003",
                    AccountId = 1,
                    MerchantId = 3,
                    DeviceId = 1,
                    Amount = 75.25m,
                    Currency = "USD",
                    Timestamp = new DateTime(2024, 11, 20, 11, 45, 0),
                    Status = "Completed",
                    CreatedAt = new DateTime(2024, 11, 20, 11, 45, 0)
                },
                new Transaction
                {
                    TransactionId = 4,
                    TransactionRef = "TXN004",
                    AccountId = 3,
                    MerchantId = 4,
                    DeviceId = 3,
                    Amount = 2500.00m,
                    Currency = "USD",
                    Timestamp = new DateTime(2024, 11, 20, 7, 0, 0),
                    Status = "Pending",
                    CreatedAt = new DateTime(2024, 11, 20, 7, 0, 0)
                },
                new Transaction
                {
                    TransactionId = 5,
                    TransactionRef = "TXN005",
                    AccountId = 1,
                    MerchantId = 1,
                    DeviceId = 1,
                    Amount = 299.99m,
                    Currency = "USD",
                    Timestamp = new DateTime(2024, 11, 19, 12, 0, 0),
                    Status = "Completed",
                    CreatedAt = new DateTime(2024, 11, 19, 12, 0, 0)
                },
                new Transaction
                {
                    TransactionId = 6,
                    TransactionRef = "TXN006",
                    AccountId = 4,
                    MerchantId = 5,
                    DeviceId = 4,
                    Amount = 450.00m,
                    Currency = "USD",
                    Timestamp = new DateTime(2024, 11, 18, 15, 30, 0),
                    Status = "Completed",
                    CreatedAt = new DateTime(2024, 11, 18, 15, 30, 0)
                }
            );

            // Seed FraudAlerts
            modelBuilder.Entity<FraudAlert>().HasData(
                new FraudAlert
                {
                    AlertId = 1,
                    TransactionId = 2,
                    AlertLevel = "High",
                    Status = "Open",
                    AssignedTo = 1,
                    CreatedAt = new DateTime(2024, 11, 20, 11, 30, 0)
                },
                new FraudAlert
                {
                    AlertId = 2,
                    TransactionId = 4,
                    AlertLevel = "Medium",
                    Status = "Under Review",
                    AssignedTo = 2,
                    CreatedAt = new DateTime(2024, 11, 20, 7, 0, 0)
                },
                new FraudAlert
                {
                    AlertId = 3,
                    TransactionId = 2,
                    AlertLevel = "High",
                    Status = "Open",
                    AssignedTo = null,
                    CreatedAt = new DateTime(2024, 11, 20, 11, 35, 0)
                }
            );

            // Seed ModelVersions
            modelBuilder.Entity<ModelVersion>().HasData(
                new ModelVersion { ModelVersionId = 1, Name = "Fraud Detection v1", Version = "1.0.0", Metrics = "{\"accuracy\": 0.95}", FileLocation = "/models/v1.pkl", CreatedAt = new DateTime(2024, 1, 1) },
                new ModelVersion { ModelVersionId = 2, Name = "Fraud Detection v2", Version = "2.0.0", Metrics = "{\"accuracy\": 0.97}", FileLocation = "/models/v2.pkl", CreatedAt = new DateTime(2024, 6, 1) }
            );
        }
    }
}