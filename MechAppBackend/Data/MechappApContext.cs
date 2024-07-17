using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MechAppBackend.MechAppApModels;

namespace MechAppBackend.Data
{
    public partial class MechappApContext : DbContext
    {
        public MechappApContext()
        {
        }

        public MechappApContext(DbContextOptions<MechappApContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<SessionToken> SessionTokens { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UsersRole> UsersRoles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySQL("Server=213.222.222.170;Database=mechappAp;Uid=AppAdmin;Pwd=Entropia13!;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasColumnType("text");
            });

            modelBuilder.Entity<SessionToken>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ExpireDate).HasColumnType("datetime");

                entity.Property(e => e.Token).HasColumnType("text");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Email).HasColumnType("text");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Login).HasColumnType("text");

                entity.Property(e => e.Password).HasColumnType("text");

                entity.Property(e => e.Phone).HasColumnType("text");

                entity.Property(e => e.Salt).HasColumnType("text");
            });

            modelBuilder.Entity<UsersRole>(entity =>
            {
                entity.HasIndex(e => e.RolesId, "RolesID");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.RolesId).HasColumnName("RolesID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Roles)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.RolesId)
                    .HasConstraintName("UsersRoles_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("UsersRoles_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
