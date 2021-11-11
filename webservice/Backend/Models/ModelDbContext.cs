using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Backend.Models
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

        public virtual DbSet<CardGroup> CardGroup { get; set; }
        public virtual DbSet<CardStatuses> CardStatuses { get; set; }
        public virtual DbSet<CardTypes> CardTypes { get; set; }
        public virtual DbSet<Cards> Cards { get; set; }
        public virtual DbSet<Changers> Changers { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<DeviceTypes> DeviceTypes { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<EventChangerKind> EventChangerKind { get; set; }
        public virtual DbSet<EventIncrease> EventIncrease { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<OperationTypes> OperationTypes { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<Regions> Regions { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<UserWash> UserWash { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Wash> Wash { get; set; }
        public virtual DbSet<WashGroup> WashGroup { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=WashCompany;Trusted_Connection=True;");
                //optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Initial Catalog=WashCompany;User Id=sa; Password=ora4paSS");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CardGroup>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Idcard).HasColumnName("IDCard");

                entity.Property(e => e.Idgroup).HasColumnName("IDGroup");

                entity.HasOne(d => d.IdcardNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idcard)
                    .HasConstraintName("FK_CardGroup_Cards");

                entity.HasOne(d => d.IdgroupNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idgroup)
                    .HasConstraintName("FK_CardGroup_Group");
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
            });

            modelBuilder.Entity<Changers>(entity =>
            {
                entity.HasKey(e => e.Idchanger);

                entity.Property(e => e.Idchanger).HasColumnName("IDChanger");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Changers)
                    .HasForeignKey(d => d.Iddevice)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Changers_Device");

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany(p => p.Changers)
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_Changers_Wash");
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

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Idevent);

                entity.Property(e => e.Idevent).HasColumnName("IDEvent");

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");

                entity.Property(e => e.IdeventPost).HasColumnName("IDEventPost");

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.HasOne(d => d.IdpostNavigation)
                    .WithMany(p => p.Event)
                    .HasForeignKey(d => d.Idpost)
                    .HasConstraintName("FK_Event_Point");
            });

            modelBuilder.Entity<EventChangerKind>(entity =>
            {
                entity.HasKey(e => e.IdeventChangerKind);

                entity.Property(e => e.IdeventChangerKind).HasColumnName("IDEventChangerKind");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EventIncrease>(entity =>
            {
                entity.HasKey(e => e.Idevent)
                    .HasName("PK_EventCash");

                entity.Property(e => e.Idevent)
                    .HasColumnName("IDEvent")
                    .ValueGeneratedNever();

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
            });

            modelBuilder.Entity<Groups>(entity =>
            {
                entity.HasKey(e => e.Idgroup)
                    .HasName("PK_Group");

                entity.Property(e => e.Idgroup).HasColumnName("IDGroup");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(25);
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

            modelBuilder.Entity<Posts>(entity =>
            {
                entity.HasKey(e => e.Idpost);

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Qrcode)
                    .HasColumnName("QRCode")
                    .HasMaxLength(20);

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

            modelBuilder.Entity<Roles>(entity =>
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

            modelBuilder.Entity<UserWash>(entity =>
            {
                entity.HasNoKey();

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

            modelBuilder.Entity<Users>(entity =>
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

            modelBuilder.Entity<Wash>(entity =>
            {
                entity.HasKey(e => e.Idwash);

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(e => e.Idregion).HasColumnName("IDRegion");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasOne(d => d.IdregionNavigation)
                    .WithMany(p => p.Wash)
                    .HasForeignKey(d => d.Idregion)
                    .HasConstraintName("FK_Wash_Regions");
            });

            modelBuilder.Entity<WashGroup>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Idgroup).HasColumnName("IDGroup");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.HasOne(d => d.IdgroupNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idgroup)
                    .HasConstraintName("FK_WashGroup_Group");

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_WashGroup_Wash");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
