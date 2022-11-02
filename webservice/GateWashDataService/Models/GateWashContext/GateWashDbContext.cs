using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
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
        public virtual DbSet<Collect> Collects { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<DeviceTypeAction> DeviceTypeActions { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventIncrease> EventIncreases { get; set; }
        public virtual DbSet<EventKind> EventKinds { get; set; }
        public virtual DbSet<EventKindWash> EventKindWashes { get; set; }
        public virtual DbSet<EventPayout> EventPayouts { get; set; }
        public virtual DbSet<PayEvent> PayEvents { get; set; }
        public virtual DbSet<PaySession> PaySessions { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Program> Programs { get; set; }
        public virtual DbSet<ProgramWash> ProgramWashes { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Terminal> Terminals { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserWash> UserWashes { get; set; }
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

                entity.Property(e => e.Idcard).HasColumnName("IDCard");

                entity.Property(e => e.CardNum)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Collect>(entity =>
            {
                entity.HasKey(e => e.Idcollect)
                    .HasName("PK_EventCollect_1");

                entity.ToTable("Collect");

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
                    .HasColumnType("datetime")
                    .HasColumnName("DTime");

                entity.Property(e => e.IdcollectOnPost).HasColumnName("IDCollectOnPost");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.InboxB100).HasColumnName("inbox_b100");

                entity.Property(e => e.InboxB50).HasColumnName("inbox_b50");

                entity.Property(e => e.M10).HasColumnName("m10");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Collects)
                    .HasForeignKey(d => d.Iddevice)
                    .HasConstraintName("FK_Collect_Device");
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

            modelBuilder.Entity<DeviceTypeAction>(entity =>
            {
                entity.HasKey(e => e.IddeviceType)
                    .HasName("PK_DeviceTypeAction");

                entity.Property(e => e.IddeviceType)
                    .ValueGeneratedNever()
                    .HasColumnName("IDDeviceType");

                entity.HasOne(d => d.IddeviceTypeNavigation)
                    .WithOne(p => p.DeviceTypeAction)
                    .HasForeignKey<DeviceTypeAction>(d => d.IddeviceType)
                    .HasConstraintName("FK_DeviceTypeActions_DeviceTypes");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Idevent);

                entity.ToTable("Event");

                entity.Property(e => e.Idevent).HasColumnName("IDEvent");

                entity.Property(e => e.Dtime)
                    .HasColumnType("datetime")
                    .HasColumnName("DTime");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");

                entity.Property(e => e.IdeventOnPost).HasColumnName("IDEventOnPost");

                entity.Property(e => e.Idsession).HasColumnName("IDSession");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.Iddevice)
                    .HasConstraintName("FK_Event_Device");

                entity.HasOne(d => d.IdeventKindNavigation)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.IdeventKind)
                    .HasConstraintName("FK_Event_EventKind");

                entity.HasOne(d => d.IdsessionNavigation)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.Idsession)
                    .HasConstraintName("FK_Event_Sessions");
            });

            modelBuilder.Entity<EventIncrease>(entity =>
            {
                entity.HasKey(e => e.IdpayEvent);

                entity.ToTable("EventIncrease");

                entity.Property(e => e.IdpayEvent)
                    .ValueGeneratedNever()
                    .HasColumnName("IDPayEvent");

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

            modelBuilder.Entity<EventKindWash>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EventKindWash");

                entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.HasOne(d => d.IdeventKindNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdeventKind)
                    .HasConstraintName("FK_EventKindWash_EventKind");

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_EventKindWash_Wash");
            });

            modelBuilder.Entity<EventPayout>(entity =>
            {
                entity.HasKey(e => e.IdpayEvent);

                entity.ToTable("EventPayout");

                entity.Property(e => e.IdpayEvent)
                    .ValueGeneratedNever()
                    .HasColumnName("IDPayEvent");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.B100).HasColumnName("b100");

                entity.Property(e => e.B50).HasColumnName("b50");

                entity.Property(e => e.Inbox1B50).HasColumnName("inbox_1_b50");

                entity.Property(e => e.Inbox2B50).HasColumnName("inbox_2_b50");

                entity.Property(e => e.Inbox3B100).HasColumnName("inbox_3_b100");

                entity.Property(e => e.Inbox4B100).HasColumnName("inbox_4_b100");

                entity.HasOne(d => d.IdpayEventNavigation)
                    .WithOne(p => p.EventPayout)
                    .HasForeignKey<EventPayout>(d => d.IdpayEvent)
                    .HasConstraintName("FK_EventPayout_PayEvent");
            });

            modelBuilder.Entity<PayEvent>(entity =>
            {
                entity.HasKey(e => e.IdpayEvent);

                entity.ToTable("PayEvent");

                entity.Property(e => e.IdpayEvent).HasColumnName("IDPayEvent");

                entity.Property(e => e.Dtime)
                    .HasColumnType("datetime")
                    .HasColumnName("DTime");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");

                entity.Property(e => e.IdeventOnPost).HasColumnName("IDEventOnPost");

                entity.Property(e => e.IdpaySession).HasColumnName("IDPaySession");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.PayEvents)
                    .HasForeignKey(d => d.Iddevice)
                    .HasConstraintName("FK_PayEvent_Device");

                entity.HasOne(d => d.IdeventKindNavigation)
                    .WithMany(p => p.PayEvents)
                    .HasForeignKey(d => d.IdeventKind)
                    .HasConstraintName("FK_PayEvent_EventKind");

                entity.HasOne(d => d.IdpaySessionNavigation)
                    .WithMany(p => p.PayEvents)
                    .HasForeignKey(d => d.IdpaySession)
                    .HasConstraintName("FK_PayEvent_PaySession");
            });

            modelBuilder.Entity<PaySession>(entity =>
            {
                entity.HasKey(e => e.IdpaySession);

                entity.ToTable("PaySession");

                entity.Property(e => e.IdpaySession).HasColumnName("IDPaySession");

                entity.Property(e => e.Details).HasMaxLength(100);

                entity.Property(e => e.DtimeBegin)
                    .HasColumnType("datetime")
                    .HasColumnName("DTimeBegin");

                entity.Property(e => e.DtimeEnd)
                    .HasColumnType("datetime")
                    .HasColumnName("DTimeEnd");

                entity.Property(e => e.FiscalError).HasMaxLength(100);

                entity.Property(e => e.Guid)
                    .HasMaxLength(32)
                    .HasColumnName("GUID")
                    .IsFixedLength(true);

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idprogram).HasColumnName("IDProgram");

                entity.Property(e => e.IdsessionOnPost).HasColumnName("IDSessionOnPost");

                entity.Property(e => e.Qr)
                    .HasMaxLength(100)
                    .HasColumnName("QR");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.PaySessions)
                    .HasForeignKey(d => d.Iddevice)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaySession_Device");

                entity.HasOne(d => d.IdprogramNavigation)
                    .WithMany(p => p.PaySessions)
                    .HasForeignKey(d => d.Idprogram)
                    .HasConstraintName("FK_PaySession_Program");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Idpost);

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Qrcode)
                    .HasMaxLength(25)
                    .HasColumnName("QRCode");

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

                entity.ToTable("Program");

                entity.Property(e => e.Idprogram).HasColumnName("IDProgram");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ProgramWash>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ProgramWash");

                entity.Property(e => e.Idprogram).HasColumnName("IDProgram");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.HasOne(d => d.IdprogramNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idprogram)
                    .HasConstraintName("FK_ProgramWash_Program");

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_ProgramWash_Wash");
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

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Idrole)
                    .HasName("PK_Role");

                entity.Property(e => e.Idrole).HasColumnName("IDRole");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.Idsession);

                entity.Property(e => e.Idsession).HasColumnName("IDSession");

                entity.Property(e => e.Dtime)
                    .HasColumnType("datetime")
                    .HasColumnName("DTime");

                entity.Property(e => e.Idcard).HasColumnName("IDCard");

                entity.Property(e => e.Idfunction).HasColumnName("IDFunction");

                entity.Property(e => e.IdsessoinOnWash).HasColumnName("IDSessoinOnWash");

                entity.Property(e => e.Uuid)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsFixedLength(true);

                entity.HasOne(d => d.IdcardNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Idcard)
                    .HasConstraintName("FK_Sessions_Cards");

                entity.HasOne(d => d.IdfunctionNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Idfunction)
                    .HasConstraintName("FK_Sessions_Functions");
            });

            modelBuilder.Entity<Terminal>(entity =>
            {
                entity.HasKey(e => e.Idterminal);

                entity.Property(e => e.Idterminal).HasColumnName("IDTerminal");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Qrcode)
                    .HasMaxLength(25)
                    .HasColumnName("QRCode");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Terminals)
                    .HasForeignKey(d => d.Iddevice)
                    .HasConstraintName("FK_Terminals_Device");

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany(p => p.Terminals)
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_Terminals_Wash");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Iduser);

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Idrole).HasColumnName("IDRole");

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.HasOne(d => d.IdroleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Idrole)
                    .HasConstraintName("FK_Users_Roles");
            });

            modelBuilder.Entity<UserWash>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserWash");

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.HasOne(d => d.IduserNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Iduser)
                    .HasConstraintName("FK_UserWash_Users");

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_UserWash_Wash");
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
