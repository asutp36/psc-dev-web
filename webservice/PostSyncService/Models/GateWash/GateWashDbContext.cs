using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PostSyncService.Models.GateWash
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

        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventIncrease> EventIncreases { get; set; }
        public virtual DbSet<EventKind> EventKinds { get; set; }
        public virtual DbSet<EventPayout> EventPayouts { get; set; }
        public virtual DbSet<Function> Functions { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Wash> Washes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=GateWash;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasKey(e => e.Idcard);

                entity.Property(e => e.Idcard)
                    .HasMaxLength(8)
                    .HasColumnName("IDCard")
                    .IsFixedLength(true);

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.Iddevice);

                entity.ToTable("Device");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.IddeviceType).HasColumnName("IDDeviceType");

                entity.Property(e => e.IpAddress).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(50);

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

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Idevent);

                entity.ToTable("Event");

                entity.HasIndex(e => new { e.Idpost, e.Dtime }, "AK_Event_IDPost_DTime")
                    .IsUnique();

                entity.Property(e => e.Idevent).HasColumnName("IDEvent");

                entity.Property(e => e.Dtime)
                    .HasColumnType("datetime")
                    .HasColumnName("DTime");

                entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.Property(e => e.Idsession).HasColumnName("IDSession");

                entity.HasOne(d => d.IdeventKindNavigation)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.IdeventKind)
                    .HasConstraintName("FK_Event_EventKind");

                entity.HasOne(d => d.IdpostNavigation)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.Idpost)
                    .HasConstraintName("FK_Event_Post");

                entity.HasOne(d => d.IdsessionNavigation)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.Idsession)
                    .HasConstraintName("FK_Event_Sessions");
            });

            modelBuilder.Entity<EventIncrease>(entity =>
            {
                entity.HasKey(e => e.Idevent);

                entity.ToTable("EventIncrease");

                entity.Property(e => e.Idevent)
                    .ValueGeneratedNever()
                    .HasColumnName("IDEvent");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.B100).HasColumnName("b100");

                entity.Property(e => e.B1000).HasColumnName("b1000");

                entity.Property(e => e.B200).HasColumnName("b200");

                entity.Property(e => e.B2000).HasColumnName("b2000");

                entity.Property(e => e.B50).HasColumnName("b50");

                entity.Property(e => e.B500).HasColumnName("b500");

                entity.Property(e => e.M10).HasColumnName("m10");

                entity.HasOne(d => d.IdeventNavigation)
                    .WithOne(p => p.EventIncrease)
                    .HasForeignKey<EventIncrease>(d => d.Idevent)
                    .HasConstraintName("FK_EventIncrease_Event");
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

            modelBuilder.Entity<EventPayout>(entity =>
            {
                entity.HasKey(e => e.Idevent);

                entity.ToTable("EventPayout");

                entity.Property(e => e.Idevent)
                    .ValueGeneratedNever()
                    .HasColumnName("IDEvent");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.B100).HasColumnName("b100");

                entity.Property(e => e.B50).HasColumnName("b50");

                entity.Property(e => e.StorageB100).HasColumnName("storage_b100");

                entity.Property(e => e.StorageB50).HasColumnName("storage_b50");

                entity.HasOne(d => d.IdeventNavigation)
                    .WithOne(p => p.EventPayout)
                    .HasForeignKey<EventPayout>(d => d.Idevent)
                    .HasConstraintName("FK_EventPayout_Event");
            });

            modelBuilder.Entity<Function>(entity =>
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

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Idpost);

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Qrcode)
                    .HasMaxLength(25)
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

            modelBuilder.Entity<Region>(entity =>
            {
                entity.HasKey(e => e.Idregion);

                entity.Property(e => e.Idregion).HasColumnName("IDRegion");

                entity.Property(e => e.Idcompany).HasColumnName("IDCompany");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.Idsession);

                entity.Property(e => e.Idsession).HasColumnName("IDSession");

                entity.Property(e => e.Dtime)
                    .HasColumnType("datetime")
                    .HasColumnName("DTime");

                entity.Property(e => e.Idcard)
                    .IsRequired()
                    .HasMaxLength(8)
                    .HasColumnName("IDCard")
                    .IsFixedLength(true);

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idfunction).HasColumnName("IDFunction");

                entity.Property(e => e.IdsessoinOnWash).HasColumnName("IDSessoinOnWash");

                entity.Property(e => e.Uuid)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Wash>(entity =>
            {
                entity.HasKey(e => e.Idwash);

                entity.ToTable("Wash");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Idregion).HasColumnName("IDRegion");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasOne(d => d.IdregionNavigation)
                    .WithMany(p => p.Washes)
                    .HasForeignKey(d => d.Idregion)
                    .HasConstraintName("FK_Wash_Regions");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
