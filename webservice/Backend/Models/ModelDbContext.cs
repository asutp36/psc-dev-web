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

        public virtual DbSet<CardStatuses> CardStatuses { get; set; }
        public virtual DbSet<CardTypes> CardTypes { get; set; }
        public virtual DbSet<Cards> Cards { get; set; }
        public virtual DbSet<Changers> Changers { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<DeviceTypes> DeviceTypes { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<EventIncrease> EventIncrease { get; set; }
        public virtual DbSet<NumsMobileCards> NumsMobileCards { get; set; }
        public virtual DbSet<OperationTypes> OperationTypes { get; set; }
        public virtual DbSet<Operations> Operations { get; set; }
        public virtual DbSet<Owners> Owners { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersAvailableWash> UsersAvailableWash { get; set; }
        public virtual DbSet<Wash> Wash { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=WashCompany;Trusted_Connection=True;");
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Initial Catalog=WashCompany;User Id=sa; Password=ora4paSS");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

                entity.Property(e => e.M10).HasColumnName("m10");

                entity.HasOne(d => d.IdeventNavigation)
                    .WithOne(p => p.EventIncrease)
                    .HasForeignKey<EventIncrease>(d => d.Idevent)
                    .HasConstraintName("FK_EventCash_Event");
            });

            modelBuilder.Entity<NumsMobileCards>(entity =>
            {
                entity.HasKey(e => e.Num);

                entity.Property(e => e.Num).HasMaxLength(20);
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

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.IdoperationType).HasColumnName("IDOperationType");

                entity.Property(e => e.LocalizedId).HasColumnName("LocalizedID");

                entity.HasOne(d => d.IdcardNavigation)
                    .WithMany(p => p.Operations)
                    .HasForeignKey(d => d.Idcard)
                    .HasConstraintName("FK_Operations_Cards");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Operations)
                    .HasForeignKey(d => d.Iddevice)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Operations_Device");

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

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

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

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Iduser);

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UsersAvailableWash>(entity =>
            {
                entity.HasKey(e => e.IduserAvailableWash);

                entity.Property(e => e.IduserAvailableWash).HasColumnName("IDUserAvailableWash");

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.HasOne(d => d.IduserNavigation)
                    .WithMany(p => p.UsersAvailableWash)
                    .HasForeignKey(d => d.Iduser)
                    .HasConstraintName("FK_UsersAvailableWash_Users");

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany(p => p.UsersAvailableWash)
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_UsersAvailableWash_Wash");
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
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
