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

        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UsersRole> UsersRoles { get; set; } = null!;
        public virtual DbSet<UsersToken> UsersTokens { get; set; } = null!;
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
