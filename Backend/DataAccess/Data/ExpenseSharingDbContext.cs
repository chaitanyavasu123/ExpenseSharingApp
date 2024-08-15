using Microsoft.EntityFrameworkCore;
using Models;
using System;

namespace DataAccess.Data
{
    public class ExpenseSharingDbContext : DbContext
    {
        public ExpenseSharingDbContext(DbContextOptions<ExpenseSharingDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseShare> ExpenseShares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Drop existing tables (for development purposes only)
            //modelBuilder.Entity<User>().ToTable("Users").Metadata.SetIsTableExcludedFromMigrations(false);
            //modelBuilder.Entity<Group>().ToTable("Groups").Metadata.SetIsTableExcludedFromMigrations(false);
            //modelBuilder.Entity<GroupMember>().ToTable("GroupMembers").Metadata.SetIsTableExcludedFromMigrations(false);
            //modelBuilder.Entity<Expense>().ToTable("Expenses").Metadata.SetIsTableExcludedFromMigrations(false);
            //modelBuilder.Entity<ExpenseShare>().ToTable("ExpenseShares").Metadata.SetIsTableExcludedFromMigrations(false);

            // Configure GroupMember relationships
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new { gm.GroupId, gm.UserId });

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ExpenseShare relationships
            // Configure ExpenseShare primary key
            modelBuilder.Entity<ExpenseShare>()
                .HasKey(es => es.ExpenseShareId);

            modelBuilder.Entity<ExpenseShare>()
                .Property(es => es.ExpenseShareId)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<ExpenseShare>()
                .HasOne(es => es.Expense)
                .WithMany(e => e.ExpenseShares)
                .HasForeignKey(es => es.ExpenseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExpenseShare>()
                .HasOne(es => es.User)
                .WithMany(u => u.ExpenseShares)
                .HasForeignKey(es => es.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Expense relationships
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.PaidBy)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.PaidById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Group)
                .WithMany(g => g.Expenses)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Name = "Admin One",
                    Email = "admin@example.com",
                    Password = "Admin@123", // In a real application, passwords should be hashed
                    Role = "Admin",
                    AmountOwed = 0,
                    AmountOwedTo = 0
                },
                new User
                {
                    UserId = 2,
                    Name = "User One",
                    Email = "user1@example.com",
                    Password = "User1@123", // In a real application, passwords should be hashed
                    Role = "User",
                    AmountOwed = 100,
                    AmountOwedTo = 0
                },
                new User
                {
                    UserId = 3,
                    Name = "User Two",
                    Email = "user2@example.com",
                    Password = "User2@123", // In a real application, passwords should be hashed
                    Role = "User",
                    AmountOwed = 100,
                    AmountOwedTo = 0
                },
                new User
                {
                    UserId = 4,
                    Name = "User Three",
                    Email = "user3@example.com",
                    Password = "User3@123", // In a real application, passwords should be hashed
                    Role = "User",
                    AmountOwed = 100,
                    AmountOwedTo = 0
                },
                 new User
                 {
                     UserId = 5,
                     Name = "User four",
                     Email = "user4@example.com",
                     Password = "User4@123", // In a real application, passwords should be hashed
                     Role = "User",
                     AmountOwed = 100,
                     AmountOwedTo = 0
                 },
                  new User
                  {
                      UserId = 6,
                      Name = "User Five",
                      Email = "user5@example.com",
                      Password = "User5@123", // In a real application, passwords should be hashed
                      Role = "User",
                      AmountOwed = 100,
                      AmountOwedTo = 0
                  },
                   new User
                   {
                       UserId = 7,
                       Name = "User Six",
                       Email = "user6@example.com",
                       Password = "User6@123", // In a real application, passwords should be hashed
                       Role = "User",
                       AmountOwed = 100,
                       AmountOwedTo = 0
                   },
                    new User
                    {
                        UserId = 8,
                        Name = "User Seven",
                        Email = "user7@example.com",
                        Password = "User7@123", // In a real application, passwords should be hashed
                        Role = "User",
                        AmountOwed = 100,
                        AmountOwedTo = 0
                    },
                     new User
                     {
                         UserId = 9,
                         Name = "User Eight",
                         Email = "user8@example.com",
                         Password = "User8@123", // In a real application, passwords should be hashed
                         Role = "User",
                         AmountOwed = 100,
                         AmountOwedTo = 0
                     }
            );

            // Seed Groups
            modelBuilder.Entity<Group>().HasData(
                new Group
                {
                    GroupId = 1,
                    Name = "Group One",
                    Description="This is group One",
                    CreatedById = 2
                },
                new Group
                {
                    GroupId = 2,
                    Name = "Group Two",
                    Description="This is Group 2",
                    CreatedById = 3
                },
                new Group
                {
                    GroupId = 3,
                    Name = "Group Three",
                    Description = "This is Group 3",
                    CreatedById = 4
                }
            );

            modelBuilder.Entity<GroupMember>().HasData(
        new GroupMember { GroupId = 1, UserId = 2 },
        new GroupMember { GroupId = 1, UserId = 3 },
        new GroupMember { GroupId = 1, UserId = 4 },
        new GroupMember { GroupId = 2, UserId = 3 },
        new GroupMember { GroupId = 2, UserId = 4 },
        new GroupMember { GroupId = 3, UserId = 4 }
    );

            base.OnModelCreating(modelBuilder);
        }
    }
}
