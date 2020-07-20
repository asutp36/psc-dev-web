﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MobileIntegration_v2.Models
{
    public partial class ModelDbContext : DbContext
    {
        public ModelDbContext()
        {
        }

        public ModelDbContext(DbContextOptions<ModelDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CardOperations> CardOperations { get; set; }
        public virtual DbSet<CardStatuses> CardStatuses { get; set; }
        public virtual DbSet<CardTypes> CardTypes { get; set; }
        public virtual DbSet<Cards> Cards { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<DeviceTypes> DeviceTypes { get; set; }
        public virtual DbSet<OperationTypes> OperationTypes { get; set; }
        public virtual DbSet<Operations> Operations { get; set; }
        public virtual DbSet<Owners> Owners { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=WashCompany;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CardOperations>(entity =>
            {
                entity.HasKey(e => e.IdcardOpration);

                entity.Property(e => e.IdcardOpration).HasColumnName("IDCardOpration");

                entity.Property(e => e.CardNum)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idstate).HasColumnName("IDState");
            });

            modelBuilder.Entity<CardStatuses>(entity =>
            {
                entity.HasKey(e => e.IdcardStatus);

                entity.Property(e => e.IdcardStatus).HasColumnName("IDCardStatus");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CardTypes>(entity =>
            {
                entity.HasKey(e => e.IdcardType);

                entity.Property(e => e.IdcardType).HasColumnName("IDCardType");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Cards>(entity =>
            {
                entity.HasKey(e => e.Idcard);

                entity.Property(e => e.Idcard).HasColumnName("IDCard");

                entity.Property(e => e.CardNum)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.IdcardStatus).HasColumnName("IDCardStatus");

                entity.Property(e => e.IdcardType).HasColumnName("IDCardType");

                entity.Property(e => e.Idowner).HasColumnName("IDOwner");

                entity.Property(e => e.LocalizedId).HasColumnName("LocalizedID");

                entity.HasOne(d => d.IdcardStatusNavigation)
                    .WithMany(p => p.Cards)
                    .HasForeignKey(d => d.IdcardStatus)
                    .HasConstraintName("FK_Cards_CardStatuses");

                entity.HasOne(d => d.IdcardTypeNavigation)
                    .WithMany(p => p.Cards)
                    .HasForeignKey(d => d.IdcardType)
                    .HasConstraintName("FK_Cards_CardTypes");

                entity.HasOne(d => d.IdownerNavigation)
                    .WithMany(p => p.Cards)
                    .HasForeignKey(d => d.Idowner)
                    .HasConstraintName("FK_Cards_Owners");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.Iddevice);

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.IddeviceType).HasColumnName("IDDeviceType");

                entity.Property(e => e.IpAddress).HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ServerId).HasColumnName("ServerID");

                entity.HasOne(d => d.IddeviceTypeNavigation)
                    .WithMany(p => p.Device)
                    .HasForeignKey(d => d.IddeviceType)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Device_DeviceTypes");
            });

            modelBuilder.Entity<DeviceTypes>(entity =>
            {
                entity.HasKey(e => e.IddeviceType);

                entity.Property(e => e.IddeviceType).HasColumnName("IDDeviceType");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<OperationTypes>(entity =>
            {
                entity.HasKey(e => e.IdoperationType);

                entity.Property(e => e.IdoperationType).HasColumnName("IDOperationType");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Operations>(entity =>
            {
                entity.HasKey(e => e.Idoperation);

                entity.Property(e => e.Idoperation).HasColumnName("IDOperation");

                entity.Property(e => e.Details).HasMaxLength(500);

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Functions).HasMaxLength(500);

                entity.Property(e => e.Idcard).HasColumnName("IDCard");

                entity.Property(e => e.IdoperationType).HasColumnName("IDOperationType");

                entity.Property(e => e.Idpsc).HasColumnName("IDPsc");

                entity.Property(e => e.LocalizedId).HasColumnName("LocalizedID");

                entity.HasOne(d => d.IdcardNavigation)
                    .WithMany(p => p.Operations)
                    .HasForeignKey(d => d.Idcard)
                    .HasConstraintName("FK_Operations_Cards");

                entity.HasOne(d => d.IdoperationTypeNavigation)
                    .WithMany(p => p.Operations)
                    .HasForeignKey(d => d.IdoperationType)
                    .HasConstraintName("FK_Operations_OperationTypes");
            });

            modelBuilder.Entity<Owners>(entity =>
            {
                entity.HasKey(e => e.Idowner)
                    .HasName("PK_Clients");

                entity.Property(e => e.Idowner).HasColumnName("IDOwner");

                entity.Property(e => e.LocalizedId).HasColumnName("LocalizedID");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Posts>(entity =>
            {
                entity.HasKey(e => e.Idpost);

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Qrcode)
                    .HasColumnName("QRCode")
                    .HasMaxLength(20);

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.Iddevice)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Posts_Device");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
