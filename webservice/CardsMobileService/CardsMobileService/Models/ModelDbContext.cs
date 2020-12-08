using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CardsMobileService.Models
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
        public virtual DbSet<CardGroup> CardGroups { get; set; }
        public virtual DbSet<CardStatus> CardStatuses { get; set; }
        public virtual DbSet<CardType> CardTypes { get; set; }
        public virtual DbSet<Changer> Changers { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<NumsMobileCard> NumsMobileCards { get; set; }
        public virtual DbSet<Operation> Operations { get; set; }
        public virtual DbSet<OperationType> OperationTypes { get; set; }
        public virtual DbSet<Owner> Owners { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Wash> Washes { get; set; }
        public virtual DbSet<WashGroup> WashGroups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=WashCompany;Trusted_Connection=True;");
                //optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Initial Catalog=WashCompany;User Id=sa; Password=ora4paSS");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

                entity.HasOne(d => d.IdownerNavigation)
                    .WithMany(p => p.Cards)
                    .HasForeignKey(d => d.Idowner)
                    .HasConstraintName("FK_Cards_Owners");
            });

            modelBuilder.Entity<CardGroup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CardGroup");

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

            modelBuilder.Entity<Changer>(entity =>
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

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Idgroup);

                entity.ToTable("Group");

                entity.Property(e => e.Idgroup).HasColumnName("IDGroup");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(25);
            });

            modelBuilder.Entity<NumsMobileCard>(entity =>
            {
                entity.HasKey(e => e.Num);

                entity.Property(e => e.Num).HasMaxLength(20);
            });

            modelBuilder.Entity<Operation>(entity =>
            {
                entity.HasKey(e => e.Idoperation);

                entity.Property(e => e.Idoperation).HasColumnName("IDOperation");

                entity.Property(e => e.Details).HasMaxLength(500);

                entity.Property(e => e.Dtime)
                    .HasColumnType("datetime")
                    .HasColumnName("DTime");

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

            modelBuilder.Entity<OperationType>(entity =>
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

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.HasKey(e => e.Idowner)
                    .HasName("PK_Clients");

                entity.Property(e => e.Idowner).HasColumnName("IDOwner");

                entity.Property(e => e.LocalizedId).HasColumnName("LocalizedID");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Idpost);

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

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

            modelBuilder.Entity<Region>(entity =>
            {
                entity.HasKey(e => e.Idregion);

                entity.Property(e => e.Idregion).HasColumnName("IDRegion");

                entity.Property(e => e.Idcompany).HasColumnName("IDCompany");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
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

                entity.HasOne(d => d.IdregionNavigation)
                    .WithMany(p => p.Washes)
                    .HasForeignKey(d => d.Idregion)
                    .HasConstraintName("FK_Wash_Regions");
            });

            modelBuilder.Entity<WashGroup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("WashGroup");

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
