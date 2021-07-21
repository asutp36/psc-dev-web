using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace HangFireTest.JobHelpers.Models
{
    public partial class HangfireDbContext : DbContext
    {
        public HangfireDbContext()
        {
        }

        public HangfireDbContext(DbContextOptions<HangfireDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<WhattsAppRecipient> WhattsAppRecipients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=HangfireTest;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<WhattsAppRecipient>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ChatId)
                    .IsRequired()
                    .HasMaxLength(80)
                    .HasColumnName("chatID");

                entity.Property(e => e.WaRecipients).HasColumnName("waRecipients");

                entity.Property(e => e.WashCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("washCode");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
