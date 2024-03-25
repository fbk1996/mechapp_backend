using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MechAppBackend.ClientsModels;

namespace MechAppBackend.Data
{
    public partial class MechappClientsContext : DbContext
    {
        public MechappClientsContext()
        {
        }

        public MechappClientsContext(DbContextOptions<MechappClientsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ClientsDatum> ClientsData { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySQL("Server=213.222.222.170;Database=MechAppClients;Uid=AppAdmin;Pwd=Entropia13!;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientsDatum>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AppPrefix).HasColumnType("text");

                entity.Property(e => e.CompanyAddress).HasColumnType("text");

                entity.Property(e => e.CompanyCity).HasColumnType("text");

                entity.Property(e => e.CompanyEmail).HasColumnType("text");

                entity.Property(e => e.CompanyName).HasColumnType("text");

                entity.Property(e => e.CompanyPhone).HasColumnType("text");

                entity.Property(e => e.CompanyPostcode).HasColumnType("text");

                entity.Property(e => e.CompanyUserDataEmail).HasColumnType("text");

                entity.Property(e => e.DateEnd).HasColumnType("datetime");

                entity.Property(e => e.Dbdatabase)
                    .HasColumnType("text")
                    .HasColumnName("DBDatabase");

                entity.Property(e => e.Dbhost)
                    .HasColumnType("text")
                    .HasColumnName("DBhost");

                entity.Property(e => e.Dbpassword)
                    .HasColumnType("text")
                    .HasColumnName("DBPassword");

                entity.Property(e => e.Dbuser)
                    .HasColumnType("text")
                    .HasColumnName("DBUser");

                entity.Property(e => e.EmailLogoUrl).HasColumnType("text");

                entity.Property(e => e.LoginUrl).HasColumnType("text");

                entity.Property(e => e.SecureKey).HasColumnType("text");

                entity.Property(e => e.SmsApiSender).HasColumnType("text");

                entity.Property(e => e.SmsApiToken).HasColumnType("text");

                entity.Property(e => e.SmtpHost).HasColumnType("text");

                entity.Property(e => e.SmtpPassword).HasColumnType("text");

                entity.Property(e => e.SmtpPort).HasColumnType("text");

                entity.Property(e => e.SmtpUser).HasColumnType("text");

                entity.Property(e => e.Subscription).HasColumnType("text");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
