﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GateWashSyncService.Models.GateWash
{
    public partial class GateWashDbContext : DbContext
    {
        public GateWashDbContext()
        {
        }

        public GateWashDbContext(DbContextOptions<GateWashDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cards> Cards { get; set; }
        public virtual DbSet<Collect> Collect { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<DeviceTypes> DeviceTypes { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<EventIncrease> EventIncrease { get; set; }
        public virtual DbSet<EventKind> EventKind { get; set; }
        public virtual DbSet<EventKindWashFee> EventKindWashFee { get; set; }
        public virtual DbSet<EventPayout> EventPayout { get; set; }
        public virtual DbSet<PayEvent> PayEvent { get; set; }
        public virtual DbSet<PaySession> PaySession { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<Program> Program { get; set; }
        public virtual DbSet<Regions> Regions { get; set; }
        public virtual DbSet<Sessions> Sessions { get; set; }
        public virtual DbSet<Terminals> Terminals { get; set; }
        public virtual DbSet<Wash> Wash { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=GateWash;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cards>(entity =>
            {
                entity.HasKey(e => e.Idcard);

                entity.Property(e => e.Idcard).HasColumnName("IDCard");

                entity.Property(e => e.CardNum)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Collect>(entity =>
            {
                entity.HasKey(e => e.Idcollect)
                    .HasName("PK_EventCollect_1");

                entity.Property(e => e.Idcollect).HasColumnName("IDCollect");

                entity.Property(e => e.B100).HasColumnName("b100");

                entity.Property(e => e.B1000).HasColumnName("b1000");

                entity.Property(e => e.B200).HasColumnName("b200");

                entity.Property(e => e.B2000).HasColumnName("b2000");

                entity.Property(e => e.B50).HasColumnName("b50");

                entity.Property(e => e.B500).HasColumnName("b500");

                entity.Property(e => e.BoxB100).HasColumnName("box_b100");

                entity.Property(e => e.BoxB50).HasColumnName("box_b50");

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdcollectOnPost).HasColumnName("IDCollectOnPost");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.InboxB100).HasColumnName("inbox_b100");

                entity.Property(e => e.InboxB50).HasColumnName("inbox_b50");

                entity.Property(e => e.M10).HasColumnName("m10");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Collect)
                    .HasForeignKey(d => d.Iddevice)
                    .HasConstraintName("FK_Collect_Device");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.Iddevice);

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.IddeviceType).HasColumnName("IDDeviceType");

                entity.Property(e => e.IpAddress).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(50);

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

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Idevent);

                entity.Property(e => e.Idevent).HasColumnName("IDEvent");

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");

                entity.Property(e => e.IdeventOnPost).HasColumnName("IDEventOnPost");

                entity.Property(e => e.Idsession).HasColumnName("IDSession");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Event)
                    .HasForeignKey(d => d.Iddevice)
                    .HasConstraintName("FK_Event_Device");

                entity.HasOne(d => d.IdeventKindNavigation)
                    .WithMany(p => p.Event)
                    .HasForeignKey(d => d.IdeventKind)
                    .HasConstraintName("FK_Event_EventKind");

                entity.HasOne(d => d.IdsessionNavigation)
                    .WithMany(p => p.Event)
                    .HasForeignKey(d => d.Idsession)
                    .HasConstraintName("FK_Event_Sessions");
            });

            modelBuilder.Entity<EventIncrease>(entity =>
            {
                entity.HasKey(e => e.IdpayEvent);

                entity.Property(e => e.IdpayEvent)
                    .HasColumnName("IDPayEvent")
                    .ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.B100).HasColumnName("b100");

                entity.Property(e => e.B1000).HasColumnName("b1000");

                entity.Property(e => e.B200).HasColumnName("b200");

                entity.Property(e => e.B2000).HasColumnName("b2000");

                entity.Property(e => e.B50).HasColumnName("b50");

                entity.Property(e => e.B500).HasColumnName("b500");

                entity.Property(e => e.M10).HasColumnName("m10");

                entity.Property(e => e.Profit).HasColumnName("profit");

                entity.HasOne(d => d.IdpayEventNavigation)
                    .WithOne(p => p.EventIncrease)
                    .HasForeignKey<EventIncrease>(d => d.IdpayEvent)
                    .HasConstraintName("FK_EventIncrease_PayEvent");
            });

            modelBuilder.Entity<EventKind>(entity =>
            {
                entity.HasKey(e => e.IdeventKind);

                entity.HasComment("Типы событий");

                entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EventKindWashFee>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.HasOne(d => d.IdeventKindNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdeventKind)
                    .HasConstraintName("FK_EventKindWashFee_EventKind");

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_EventKindWashFee_Wash");
            });

            modelBuilder.Entity<EventPayout>(entity =>
            {
                entity.HasKey(e => e.IdpayEvent);

                entity.Property(e => e.IdpayEvent)
                    .HasColumnName("IDPayEvent")
                    .ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.B100).HasColumnName("b100");

                entity.Property(e => e.B50).HasColumnName("b50");

                entity.Property(e => e.Inbox1B50).HasColumnName("inbox_1_b50");

                entity.Property(e => e.Inbox2B50).HasColumnName("inbox_2_b50");

                entity.Property(e => e.Inbox3B100).HasColumnName("inbox_3_b100");

                entity.Property(e => e.Inbox4B100).HasColumnName("inbox_4_b100");

                entity.Property(e => e.Inbox5M10).HasColumnName("inbox_5_m10");

                entity.Property(e => e.M10).HasColumnName("m10");

                entity.HasOne(d => d.IdpayEventNavigation)
                    .WithOne(p => p.EventPayout)
                    .HasForeignKey<EventPayout>(d => d.IdpayEvent)
                    .HasConstraintName("FK_EventPayout_PayEvent");
            });

            modelBuilder.Entity<PayEvent>(entity =>
            {
                entity.HasKey(e => e.IdpayEvent);

                entity.Property(e => e.IdpayEvent).HasColumnName("IDPayEvent");

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");

                entity.Property(e => e.IdeventOnPost).HasColumnName("IDEventOnPost");

                entity.Property(e => e.IdpaySession).HasColumnName("IDPaySession");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.PayEvent)
                    .HasForeignKey(d => d.Iddevice)
                    .HasConstraintName("FK_PayEvent_Device");

                entity.HasOne(d => d.IdeventKindNavigation)
                    .WithMany(p => p.PayEvent)
                    .HasForeignKey(d => d.IdeventKind)
                    .HasConstraintName("FK_PayEvent_EventKind");

                entity.HasOne(d => d.IdpaySessionNavigation)
                    .WithMany(p => p.PayEvent)
                    .HasForeignKey(d => d.IdpaySession)
                    .HasConstraintName("FK_PayEvent_PaySession");
            });

            modelBuilder.Entity<PaySession>(entity =>
            {
                entity.HasKey(e => e.IdpaySession);

                entity.Property(e => e.IdpaySession).HasColumnName("IDPaySession");

                entity.Property(e => e.Details).HasMaxLength(100);

                entity.Property(e => e.DtimeBegin)
                    .HasColumnName("DTimeBegin")
                    .HasColumnType("datetime");

                entity.Property(e => e.DtimeEnd)
                    .HasColumnName("DTimeEnd")
                    .HasColumnType("datetime");

                entity.Property(e => e.FiscalError).HasMaxLength(100);

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasMaxLength(32)
                    .IsFixedLength();

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idprogram).HasColumnName("IDProgram");

                entity.Property(e => e.IdsessionOnPost).HasColumnName("IDSessionOnPost");

                entity.Property(e => e.Qr)
                    .HasColumnName("QR")
                    .HasMaxLength(100);

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.PaySession)
                    .HasForeignKey(d => d.Iddevice)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaySession_Device");

                entity.HasOne(d => d.IdprogramNavigation)
                    .WithMany(p => p.PaySession)
                    .HasForeignKey(d => d.Idprogram)
                    .HasConstraintName("FK_PaySession_Program");
            });

            modelBuilder.Entity<Posts>(entity =>
            {
                entity.HasKey(e => e.Idpost);

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Qrcode)
                    .HasColumnName("QRCode")
                    .HasMaxLength(25);

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.Iddevice)
                    .HasConstraintName("FK_Posts_Device");

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_Posts_Wash");
            });

            modelBuilder.Entity<Program>(entity =>
            {
                entity.HasKey(e => e.Idprogram)
                    .HasName("PK_Functions");

                entity.Property(e => e.Idprogram).HasColumnName("IDProgram");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Regions>(entity =>
            {
                entity.HasKey(e => e.Idregion);

                entity.Property(e => e.Idregion).HasColumnName("IDRegion");

                entity.Property(e => e.Idcompany).HasColumnName("IDCompany");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Sessions>(entity =>
            {
                entity.HasKey(e => e.Idsession);

                entity.Property(e => e.Idsession).HasColumnName("IDSession");

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Idcard).HasColumnName("IDCard");

                entity.Property(e => e.Idfunction).HasColumnName("IDFunction");

                entity.Property(e => e.IdsessoinOnWash).HasColumnName("IDSessoinOnWash");

                entity.Property(e => e.Uuid)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsFixedLength();

                entity.HasOne(d => d.IdcardNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Idcard)
                    .HasConstraintName("FK_Sessions_Cards");

                entity.HasOne(d => d.IdfunctionNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Idfunction)
                    .HasConstraintName("FK_Sessions_Functions");
            });

            modelBuilder.Entity<Terminals>(entity =>
            {
                entity.HasKey(e => e.Idterminal);

                entity.Property(e => e.Idterminal).HasColumnName("IDTerminal");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Qrcode)
                    .HasColumnName("QRCode")
                    .HasMaxLength(25);

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Terminals)
                    .HasForeignKey(d => d.Iddevice)
                    .HasConstraintName("FK_Terminals_Device");

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany(p => p.Terminals)
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_Terminals_Wash");
            });

            modelBuilder.Entity<Wash>(entity =>
            {
                entity.HasKey(e => e.Idwash);

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Idregion).HasColumnName("IDRegion");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasOne(d => d.IdregionNavigation)
                    .WithMany(p => p.Wash)
                    .HasForeignKey(d => d.Idregion)
                    .HasConstraintName("FK_Wash_Regions");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
