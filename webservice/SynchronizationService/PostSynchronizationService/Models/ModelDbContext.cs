using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PostSynchronizationService.Models
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

        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<CardStatus> CardStatuses { get; set; }
        public virtual DbSet<CardType> CardTypes { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventIncrease> EventIncreases { get; set; }
        public virtual DbSet<EventKind> EventKinds { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<PostSession> PostSessions { get; set; }
        public virtual DbSet<Wash> Washes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=WashCompany;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Card>(entity =>
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
            });

            modelBuilder.Entity<CardStatus>(entity =>
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

            modelBuilder.Entity<CardType>(entity =>
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

            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.Iddevice);

                entity.ToTable("Device");

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
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.IddeviceType)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Device_DeviceTypes");
            });

            modelBuilder.Entity<DeviceType>(entity =>
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

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Idevent);

                entity.ToTable("Event");

                entity.Property(e => e.Idevent).HasColumnName("IDEvent");

                entity.Property(e => e.Dtime)
                    .HasColumnType("datetime")
                    .HasColumnName("DTime");

                entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");

                entity.Property(e => e.IdeventPost).HasColumnName("IDEventPost");

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.HasOne(d => d.IdeventKindNavigation)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.IdeventKind)
                    .HasConstraintName("FK_Event_EventKind");

                entity.HasOne(d => d.IdpostNavigation)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.Idpost)
                    .HasConstraintName("FK_Event_Point");
            });

            modelBuilder.Entity<EventIncrease>(entity =>
            {
                entity.HasKey(e => e.Idevent)
                    .HasName("PK_EventCash");

                entity.ToTable("EventIncrease");

                entity.Property(e => e.Idevent)
                    .ValueGeneratedNever()
                    .HasColumnName("IDEvent");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.B10).HasColumnName("b10");

                entity.Property(e => e.B100).HasColumnName("b100");

                entity.Property(e => e.B200).HasColumnName("b200");

                entity.Property(e => e.B50).HasColumnName("b50");

                entity.Property(e => e.Balance).HasColumnName("balance");

                entity.Property(e => e.IdpostSession).HasColumnName("IDPostSession");

                entity.Property(e => e.M10).HasColumnName("m10");

                entity.HasOne(d => d.IdeventNavigation)
                    .WithOne(p => p.EventIncrease)
                    .HasForeignKey<EventIncrease>(d => d.Idevent)
                    .HasConstraintName("FK_EventCash_Event");

                entity.HasOne(d => d.IdpostSessionNavigation)
                    .WithMany(p => p.EventIncreases)
                    .HasForeignKey(d => d.IdpostSession)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_EventIncrease_PostSession");
            });

            modelBuilder.Entity<EventKind>(entity =>
            {
                entity.HasKey(e => e.IdeventKind);

                entity.ToTable("EventKind");

                entity.HasComment("Типы событий");

                entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Idpost);

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Qrcode)
                    .HasMaxLength(20)
                    .HasColumnName("QRCode");

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

            modelBuilder.Entity<PostSession>(entity =>
            {
                entity.HasKey(e => e.IdpostSession);

                entity.ToTable("PostSession");

                entity.Property(e => e.IdpostSession).HasColumnName("IDPostSession");

                entity.Property(e => e.FiscalError).HasMaxLength(150);

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.Property(e => e.IdsessionOnPost).HasColumnName("IDSessionOnPost");

                entity.Property(e => e.Qr)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("QR");

                entity.Property(e => e.StartDtime)
                    .HasColumnType("datetime")
                    .HasColumnName("StartDTime");

                entity.Property(e => e.StopDtime)
                    .HasColumnType("datetime")
                    .HasColumnName("StopDTime");
            });

            modelBuilder.Entity<Wash>(entity =>
            {
                entity.HasKey(e => e.Idwash);

                entity.ToTable("Wash");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(e => e.Idregion).HasColumnName("IDRegion");

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
