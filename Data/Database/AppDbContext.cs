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
                    .SetBasePath(AppContext.BaseDirectory) // Använd AppContext.BaseDirectory istället för Directory.GetCurrentDirectory()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite Key for Order
            modelBuilder.Entity<Order>()
                .HasKey(o => new { o.CustomerID, o.ServiceID });

            // ProjectLeader Primary Key
            modelBuilder.Entity<ProjectLeader>()
                .Property(pl => pl.ProjectLeaderID)
                .ValueGeneratedOnAdd();

            // Project Primary Key
            modelBuilder.Entity<Project>()
                .Property(p => p.ProjectID)
                .ValueGeneratedOnAdd();

            // Customer Primary Key
            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerID)
                .ValueGeneratedOnAdd();

            // Service Primary Key
            modelBuilder.Entity<Service>()
                .Property(s => s.ServiceID)
                .ValueGeneratedOnAdd();

            // Summary Primary Key
            modelBuilder.Entity<Summary>()
                .Property(s => s.SummaryID)
                .ValueGeneratedOnAdd();

            // Relationships
            modelBuilder.Entity<Project>()
                .HasOne(p => p.ProjectLeader)
                .WithMany(pl => pl.Projects)
                .HasForeignKey(p => p.ProjectLeaderID);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerID);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Service)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.ServiceID);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Project)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProjectID);

            modelBuilder.Entity<Summary>()
                .HasOne(s => s.Project)
                .WithOne(p => p.Summary)
                .HasForeignKey<Summary>(s => s.ProjectID);
        }
    }
}
