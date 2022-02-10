using System;
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
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<DeviceTypes> DeviceTypes { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<EventCollect> EventCollect { get; set; }
        public virtual DbSet<EventIncrease> EventIncrease { get; set; }
        public virtual DbSet<EventKind> EventKind { get; set; }
        public virtual DbSet<EventPayout> EventPayout { get; set; }
        public virtual DbSet<Functions> Functions { get; set; }
        public virtual DbSet<PayEvent> PayEvent { get; set; }
        public virtual DbSet<PaySession> PaySession { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<Regions> Regions { get; set; }
        public virtual DbSet<Sessions> Sessions { get; set; }
        public virtual DbSet<Wash> Wash { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=GateWash;Trusted_Connection=True;");
                //optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Initial Catalog=GateWash;User Id=sa; Password=ora4paSS");
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

            modelBuilder.Entity<EventCollect>(entity =>
            {
                entity.HasKey(e => e.IdpayEvent);

                entity.Property(e => e.IdpayEvent)
                    .HasColumnName("IDPayEvent")
                    .ValueGeneratedNever();

                entity.Property(e => e.B100).HasColumnName("b100");

                entity.Property(e => e.B1000).HasColumnName("b1000");

                entity.Property(e => e.B200).HasColumnName("b200");

                entity.Property(e => e.B2000).HasColumnName("b2000");

                entity.Property(e => e.B50).HasColumnName("b50");

                entity.Property(e => e.B500).HasColumnName("b500");

                entity.Property(e => e.BoxB100).HasColumnName("box_b100");

                entity.Property(e => e.BoxB50).HasColumnName("box_b50");

                entity.Property(e => e.InboxB100).HasColumnName("inbox_b100");

                entity.Property(e => e.InboxB50).HasColumnName("inbox_b50");

                entity.Property(e => e.M10).HasColumnName("m10");

                entity.HasOne(d => d.IdpayEventNavigation)
                    .WithOne(p => p.EventCollect)
                    .HasForeignKey<EventCollect>(d => d.IdpayEvent)
                    .HasConstraintName("FK_EventCollect_PayEvent");
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

            modelBuilder.Entity<EventPayout>(entity =>
            {
                entity.HasKey(e => e.IdpayEvent);

                entity.Property(e => e.IdpayEvent)
                    .HasColumnName("IDPayEvent")
                    .ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.B100).HasColumnName("b100");

                entity.Property(e => e.B50).HasColumnName("b50");

                entity.Property(e => e.StorageB100).HasColumnName("storage_b100");

                entity.Property(e => e.StorageB50).HasColumnName("storage_b50");

                entity.HasOne(d => d.IdpayEventNavigation)
                    .WithOne(p => p.EventPayout)
                    .HasForeignKey<EventPayout>(d => d.IdpayEvent)
                    .HasConstraintName("FK_EventPayout_PayEvent");
            });

            modelBuilder.Entity<Functions>(entity =>
            {
                entity.HasKey(e => e.Idfunction);

                entity.Property(e => e.Idfunction).HasColumnName("IDFunction");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
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

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idfunction).HasColumnName("IDFunction");

                entity.Property(e => e.IdsessionOnPost).HasColumnName("IDSessionOnPost");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.PaySession)
                    .HasForeignKey(d => d.Iddevice)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaySession_Device");

                entity.HasOne(d => d.IdfunctionNavigation)
                    .WithMany(p => p.PaySession)
                    .HasForeignKey(d => d.Idfunction)
                    .HasConstraintName("FK_PaySession_Functions");
            });

            modelBuilder.Entity<Posts>(entity =>
            {
                entity.HasKey(e => e.Idpost);

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Qrcode)
                    .HasColumnName("QRCode")
                    .HasMaxLength(25);

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.Iddevice)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Posts_Device");

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_Posts_Wash");
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
