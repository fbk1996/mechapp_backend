using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MechAppBackend.Models;

namespace MechAppBackend.Data
{
    public partial class MechAppContext : DbContext
    {
        public MechAppContext()
        {
        }

        public MechAppContext(DbContextOptions<MechAppContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CheckList> CheckLists { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Estimate> Estimates { get; set; } = null!;
        public virtual DbSet<EstimatePart> EstimateParts { get; set; } = null!;
        public virtual DbSet<EstimateService> EstimateServices { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UsersDepartment> UsersDepartments { get; set; } = null!;
        public virtual DbSet<UsersRole> UsersRoles { get; set; } = null!;
        public virtual DbSet<UsersToken> UsersTokens { get; set; } = null!;
        public virtual DbSet<UsersVehicle> UsersVehicles { get; set; } = null!;
        public virtual DbSet<ValidationCode> ValidationCodes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseMySQL("Server=213.222.222.170;Database=mechappNew;Uid=AppAdmin;Pwd=Entropia13!;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CheckList>(entity =>
            {
                entity.ToTable("CheckList");

                entity.HasIndex(e => e.OrderId, "OrderID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BrakesFrontDescription).HasColumnType("text");

                entity.Property(e => e.BrakesFrontStatus).HasDefaultValueSql("'0'");

                entity.Property(e => e.BrakesRearDescription).HasColumnType("text");

                entity.Property(e => e.BrakesRearStatus).HasDefaultValueSql("'0'");

                entity.Property(e => e.ElectricSystemDescription).HasColumnType("text");

                entity.Property(e => e.ElectricSystemStatus).HasDefaultValueSql("'0'");

                entity.Property(e => e.EngineDescription).HasColumnType("text");

                entity.Property(e => e.EngineStatus).HasDefaultValueSql("'0'");

                entity.Property(e => e.EngineSuspensionDescription).HasColumnType("text");

                entity.Property(e => e.EngineSuspensionStatus).HasDefaultValueSql("'0'");

                entity.Property(e => e.ExhoustSystemDescription).HasColumnType("text");

                entity.Property(e => e.ExhoustSystemStatus).HasDefaultValueSql("'0'");

                entity.Property(e => e.FluidLevelDescription).HasColumnType("text");

                entity.Property(e => e.FluidLevelStatus).HasDefaultValueSql("'0'");

                entity.Property(e => e.LeaksDescription).HasColumnType("text");

                entity.Property(e => e.LeaksStatus).HasDefaultValueSql("'0'");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.SuspensionFrontDescription).HasColumnType("text");

                entity.Property(e => e.SuspensionFrontStatus).HasDefaultValueSql("'0'");

                entity.Property(e => e.SuspensionRearDescription).HasColumnType("text");

                entity.Property(e => e.SuspensionRearStatus).HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.CheckLists)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("CheckList_ibfk_1");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address).HasColumnType("text");

                entity.Property(e => e.City).HasColumnType("text");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name).HasColumnType("text");

                entity.Property(e => e.Postcode).HasColumnType("text");
            });

            modelBuilder.Entity<Estimate>(entity =>
            {
                entity.HasIndex(e => e.OrderId, "OrderID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.TotalPartsPrice).HasPrecision(10);

                entity.Property(e => e.TotalPrice).HasPrecision(10);

                entity.Property(e => e.TotalServicesPrice).HasPrecision(10);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Estimates)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("Estimates_ibfk_1");
            });

            modelBuilder.Entity<EstimatePart>(entity =>
            {
                entity.HasIndex(e => e.EstimateId, "EstimateID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Ean)
                    .HasColumnType("text")
                    .HasColumnName("EAN");

                entity.Property(e => e.EstimateId).HasColumnName("EstimateID");

                entity.Property(e => e.GrossUnitPrice).HasPrecision(10);

                entity.Property(e => e.Name).HasColumnType("text");

                entity.Property(e => e.TotalPrice).HasPrecision(10);

                entity.HasOne(d => d.Estimate)
                    .WithMany(p => p.EstimateParts)
                    .HasForeignKey(d => d.EstimateId)
                    .HasConstraintName("EstimateParts_ibfk_1");
            });

            modelBuilder.Entity<EstimateService>(entity =>
            {
                entity.HasIndex(e => e.EstimateId, "EstimateID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.EstimateId).HasColumnName("EstimateID");

                entity.Property(e => e.GrossUnitPrice).HasPrecision(10);

                entity.Property(e => e.Name).HasColumnType("text");

                entity.Property(e => e.Rhwamount).HasColumnName("RHWAmount");

                entity.Property(e => e.TotalPrice).HasPrecision(10);

                entity.HasOne(d => d.Estimate)
                    .WithMany(p => p.EstimateServices)
                    .HasForeignKey(d => d.EstimateId)
                    .HasConstraintName("EstimateServices_ibfk_1");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.ClientId, "ClientID");

                entity.HasIndex(e => e.DepartmentId, "DepartmentID");

                entity.HasIndex(e => e.VehicleId, "VehicleID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClientId).HasColumnName("ClientID");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.SendDoneNotification).HasDefaultValueSql("'0'");

                entity.Property(e => e.SendStartNotification).HasDefaultValueSql("'0'");

                entity.Property(e => e.SendThanksNotification).HasDefaultValueSql("'0'");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("text");

                entity.Property(e => e.VehicleId).HasColumnName("VehicleID");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("Orders_ibfk_1");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("Orders_ibfk_3");

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("Orders_ibfk_2");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasColumnType("text");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address).HasColumnType("text");

                entity.Property(e => e.AppRole).HasColumnType("text");

                entity.Property(e => e.City).HasColumnType("text");

                entity.Property(e => e.Color).HasColumnType("text");

                entity.Property(e => e.CompanyName).HasColumnType("text");

                entity.Property(e => e.Email).HasColumnType("text");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.IsFirstLogin).HasColumnName("isFirstLogin");

                entity.Property(e => e.IsLoyalCustomer).HasColumnName("isLoyalCustomer");

                entity.Property(e => e.Lastname).HasColumnType("text");

                entity.Property(e => e.Name).HasColumnType("text");

                entity.Property(e => e.Nip)
                    .HasColumnType("text")
                    .HasColumnName("NIP");

                entity.Property(e => e.Phone).HasColumnType("text");

                entity.Property(e => e.Postcode).HasColumnType("text");

                entity.Property(e => e.Salt).HasColumnType("text");
            });

            modelBuilder.Entity<UsersDepartment>(entity =>
            {
                entity.HasIndex(e => e.DepartmentId, "DepartmentID");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.UsersDepartments)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("UsersDepartments_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersDepartments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("UsersDepartments_ibfk_2");
            });

            modelBuilder.Entity<UsersRole>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "RoleID");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("UsersRoles_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("UsersRoles_ibfk_1");
            });

            modelBuilder.Entity<UsersToken>(entity =>
            {
                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Expire).HasColumnType("datetime");

                entity.Property(e => e.Token).HasColumnType("text");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersTokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("UsersTokens_ibfk_1");
            });

            modelBuilder.Entity<UsersVehicle>(entity =>
            {
                entity.HasIndex(e => e.Owner, "Owner");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.EngineCapacity).HasColumnType("text");

                entity.Property(e => e.EngineNumber).HasColumnType("text");

                entity.Property(e => e.FuelType).HasColumnType("text");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Model).HasColumnType("text");

                entity.Property(e => e.ProduceDate).HasColumnType("text");

                entity.Property(e => e.Producer).HasColumnType("text");

                entity.Property(e => e.RegistrationNumber).HasColumnType("text");

                entity.Property(e => e.Vin)
                    .HasColumnType("text")
                    .HasColumnName("VIN");

                entity.HasOne(d => d.OwnerNavigation)
                    .WithMany(p => p.UsersVehicles)
                    .HasForeignKey(d => d.Owner)
                    .HasConstraintName("UsersVehicles_ibfk_1");
            });

            modelBuilder.Entity<ValidationCode>(entity =>
            {
                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Code).HasColumnType("text");

                entity.Property(e => e.Expire).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ValidationCodes)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("ValidationCodes_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
