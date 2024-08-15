﻿// <auto-generated />
using System;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccess.Migrations
{
    [DbContext(typeof(ExpenseSharingDbContext))]
    partial class ExpenseSharingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Models.Expense", b =>
                {
                    b.Property<int>("ExpenseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ExpenseId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("PaidById")
                        .HasColumnType("int");

                    b.HasKey("ExpenseId");

                    b.HasIndex("GroupId");

                    b.HasIndex("PaidById");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("Models.ExpenseShare", b =>
                {
                    b.Property<int>("ExpenseShareId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ExpenseShareId"));

                    b.Property<int>("ExpenseId")
                        .HasColumnType("int");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("bit");

                    b.Property<decimal>("ShareAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ExpenseShareId");

                    b.HasIndex("ExpenseId");

                    b.HasIndex("UserId");

                    b.ToTable("ExpenseShares");
                });

            modelBuilder.Entity("Models.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GroupId"));

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GroupId");

                    b.HasIndex("CreatedById");

                    b.ToTable("Groups");

                    b.HasData(
                        new
                        {
                            GroupId = 1,
                            CreatedById = 2,
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "This is group One",
                            Name = "Group One"
                        },
                        new
                        {
                            GroupId = 2,
                            CreatedById = 3,
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "This is Group 2",
                            Name = "Group Two"
                        },
                        new
                        {
                            GroupId = 3,
                            CreatedById = 4,
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "This is Group 3",
                            Name = "Group Three"
                        });
                });

            modelBuilder.Entity("Models.GroupMember", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupMembers");

                    b.HasData(
                        new
                        {
                            GroupId = 1,
                            UserId = 2
                        },
                        new
                        {
                            GroupId = 1,
                            UserId = 3
                        },
                        new
                        {
                            GroupId = 1,
                            UserId = 4
                        },
                        new
                        {
                            GroupId = 2,
                            UserId = 3
                        },
                        new
                        {
                            GroupId = 2,
                            UserId = 4
                        },
                        new
                        {
                            GroupId = 3,
                            UserId = 4
                        });
                });

            modelBuilder.Entity("Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<decimal>("AmountOwed")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("AmountOwedTo")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            AmountOwed = 0m,
                            AmountOwedTo = 0m,
                            Email = "admin@example.com",
                            Name = "Admin One",
                            Password = "Admin@123",
                            Role = "Admin"
                        },
                        new
                        {
                            UserId = 2,
                            AmountOwed = 100m,
                            AmountOwedTo = 0m,
                            Email = "user1@example.com",
                            Name = "User One",
                            Password = "User1@123",
                            Role = "User"
                        },
                        new
                        {
                            UserId = 3,
                            AmountOwed = 100m,
                            AmountOwedTo = 0m,
                            Email = "user2@example.com",
                            Name = "User Two",
                            Password = "User2@123",
                            Role = "User"
                        },
                        new
                        {
                            UserId = 4,
                            AmountOwed = 100m,
                            AmountOwedTo = 0m,
                            Email = "user3@example.com",
                            Name = "User Three",
                            Password = "User3@123",
                            Role = "User"
                        },
                        new
                        {
                            UserId = 5,
                            AmountOwed = 100m,
                            AmountOwedTo = 0m,
                            Email = "user4@example.com",
                            Name = "User four",
                            Password = "User4@123",
                            Role = "User"
                        },
                        new
                        {
                            UserId = 6,
                            AmountOwed = 100m,
                            AmountOwedTo = 0m,
                            Email = "user5@example.com",
                            Name = "User Five",
                            Password = "User5@123",
                            Role = "User"
                        },
                        new
                        {
                            UserId = 7,
                            AmountOwed = 100m,
                            AmountOwedTo = 0m,
                            Email = "user6@example.com",
                            Name = "User Six",
                            Password = "User6@123",
                            Role = "User"
                        },
                        new
                        {
                            UserId = 8,
                            AmountOwed = 100m,
                            AmountOwedTo = 0m,
                            Email = "user7@example.com",
                            Name = "User Seven",
                            Password = "User7@123",
                            Role = "User"
                        },
                        new
                        {
                            UserId = 9,
                            AmountOwed = 100m,
                            AmountOwedTo = 0m,
                            Email = "user8@example.com",
                            Name = "User Eight",
                            Password = "User8@123",
                            Role = "User"
                        });
                });

            modelBuilder.Entity("Models.Expense", b =>
                {
                    b.HasOne("Models.Group", "Group")
                        .WithMany("Expenses")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.User", "PaidBy")
                        .WithMany("Expenses")
                        .HasForeignKey("PaidById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("PaidBy");
                });

            modelBuilder.Entity("Models.ExpenseShare", b =>
                {
                    b.HasOne("Models.Expense", "Expense")
                        .WithMany("ExpenseShares")
                        .HasForeignKey("ExpenseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Models.User", "User")
                        .WithMany("ExpenseShares")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Expense");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Models.Group", b =>
                {
                    b.HasOne("Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("Models.GroupMember", b =>
                {
                    b.HasOne("Models.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.User", "User")
                        .WithMany("GroupMemberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Models.Expense", b =>
                {
                    b.Navigation("ExpenseShares");
                });

            modelBuilder.Entity("Models.Group", b =>
                {
                    b.Navigation("Expenses");

                    b.Navigation("Members");
                });

            modelBuilder.Entity("Models.User", b =>
                {
                    b.Navigation("ExpenseShares");

                    b.Navigation("Expenses");

                    b.Navigation("GroupMemberships");
                });
#pragma warning restore 612, 618
        }
    }
}
