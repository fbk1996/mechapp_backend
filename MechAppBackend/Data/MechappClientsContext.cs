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
        public virtual DbSet<Ticket> Tickets { get; set; } = null!;
        public virtual DbSet<TicketsFile> TicketsFiles { get; set; } = null!;
        public virtual DbSet<TicketsMessage> TicketsMessages { get; set; } = null!;

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

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClientEmail).HasColumnType("text");

                entity.Property(e => e.ClientLastname).HasColumnType("text");

                entity.Property(e => e.ClientName).HasColumnType("text");

                entity.Property(e => e.ClientPhone).HasColumnType("text");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.SubscriptionId).HasColumnType("text");

                entity.Property(e => e.Title).HasColumnType("text");
            });

            modelBuilder.Entity<TicketsFile>(entity =>
            {
                entity.HasIndex(e => e.MessageId, "MessageId");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasColumnType("text");

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.TicketsFiles)
                    .HasForeignKey(d => d.MessageId)
                    .HasConstraintName("TicketsFiles_ibfk_1");
            });

            modelBuilder.Entity<TicketsMessage>(entity =>
            {
                entity.HasIndex(e => e.TicketId, "TicketId");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IsNotificationSend)
                    .HasColumnName("isNotificationSend")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Message).HasColumnType("text");

                entity.Property(e => e.User).HasColumnType("text");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketsMessages)
                    .HasForeignKey(d => d.TicketId)
                    .HasConstraintName("TicketsMessages_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
