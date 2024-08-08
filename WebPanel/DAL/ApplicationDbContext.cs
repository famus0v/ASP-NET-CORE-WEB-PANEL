using Automarket.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using WebPanel.Domain.Entity;
using WebPanel.Domain.Enum;

namespace WebPanel.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<BaseTable> BaseTables { get; set; }
        public DbSet<TableElement> TableElements { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<FileModel> Files { get; set; }
        public DbSet<ColorDataInfo> ColorData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(builder =>
            {
                builder.ToTable("accounts").HasKey(x => x.Id);

                builder.HasData(new Account[]
                {
                    new Account()
                    {
                        Id = 1,
                        Name = "base_admin",
                        FullName = "Администратор",
                        Password = HashPasswordHelper.HashPassowrd("123456"),
                        Role = Role.Admin
                    }
                });

                builder.Property(x => x.Id).ValueGeneratedOnAdd();

                builder.Property(x => x.Password).IsRequired();
                builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            });


            modelBuilder.Entity<BaseTable>(builder =>
            {
                builder.ToTable("baseTable").HasKey(x => x.Id);

                builder.Property(x => x.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TableElement>(builder =>
            {
                builder.ToTable("tableElement").HasKey(x => x.Id);
                builder.Property(x => x.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
