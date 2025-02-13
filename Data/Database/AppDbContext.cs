using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Data.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<ProjectLeader> ProjectLeaders { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Summary> Summaries { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Lägg till en parameterlös konstruktor för EF Core CLI
        public AppDbContext() { }

        // Konfigurera databasanslutning om den inte redan är inställd (för EF Core CLI)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                optionsBuilder
                    .UseSqlServer(connectionString).UseLazyLoadingProxies(); // 🔹 Aktivera Lazy Loading
            }
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ **Sammansatt primärnyckel för Orders**
            modelBuilder.Entity<Order>()
                .HasKey(o => new { o.ProjectID, o.CustomerID, o.ServiceID });

            // 🔹 **Relation mellan Order och Project**
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Project)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProjectID)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 **Relation mellan Order och Customer**
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 **Relation mellan Order och Service**
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Service)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.ServiceID)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 **Primärnyckel för Project**
            modelBuilder.Entity<Project>()
                .Property(p => p.ProjectID)
                .ValueGeneratedOnAdd();

            // 🔹 **Relation mellan Project och ProjectLeader**
            modelBuilder.Entity<Project>()
                .HasOne(p => p.ProjectLeader)
                .WithMany(pl => pl.Projects)
                .HasForeignKey(p => p.ProjectLeaderID);

            // 🔹 **Primärnyckel för ProjectLeader**
            modelBuilder.Entity<ProjectLeader>()
                .Property(pl => pl.ProjectLeaderID)
                .ValueGeneratedOnAdd();

            // 🔹 **Primärnyckel för Customer**
            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerID)
                .ValueGeneratedOnAdd();

            // 🔹 **Primärnyckel för Service**
            modelBuilder.Entity<Service>()
                .Property(s => s.ServiceID)
                .ValueGeneratedOnAdd();

            // 🔹 **Primärnyckel för Summary**
            modelBuilder.Entity<Summary>()
                .Property(s => s.SummaryID)
                .ValueGeneratedOnAdd();

            // 🔹 **Relation mellan Summary och Project**
            modelBuilder.Entity<Summary>()
                .HasOne(s => s.Project)
                .WithOne(p => p.Summary)
                .HasForeignKey<Summary>(s => s.ProjectID)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ **Fix för decimalprecision**
            modelBuilder.Entity<Customer>()
                .Property(c => c.Discount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Hours)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Summary>()
                .Property(s => s.TotalHours)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Summary>()
                .Property(s => s.TotalPrice)
                .HasPrecision(18, 2);
        }


    }
}
