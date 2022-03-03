using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace MobileAppWasteSender.Models.WashCompany
{
    public partial class WashCompanyContext : DbContext
    {
        public WashCompanyContext()
        {
        }

        public WashCompanyContext(DbContextOptions<WashCompanyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<MobileSending> MobileSendings { get; set; }
        public virtual DbSet<Post> Posts { get; set; }

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
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

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

            modelBuilder.Entity<MobileSending>(entity =>
            {
                entity.HasKey(e => e.IdmobileSending);

                entity.HasIndex(e => new { e.Idcard, e.Idpost, e.DtimeStart }, "UC_MobileSending")
                    .IsUnique();

                entity.Property(e => e.IdmobileSending).HasColumnName("IDMobileSending");

                entity.Property(e => e.DtimeEnd)
                    .HasColumnType("datetime")
                    .HasColumnName("DTimeEnd");

                entity.Property(e => e.DtimeStart)
                    .HasColumnType("datetime")
                    .HasColumnName("DTimeStart");

                entity.Property(e => e.Guid)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsFixedLength(true);

                entity.Property(e => e.Idcard).HasColumnName("IDCard");

                entity.Property(e => e.Idpost).HasColumnName("IDPost");

                entity.Property(e => e.ResultMessage).HasMaxLength(80);

                entity.HasOne(d => d.IdpostNavigation)
                    .WithMany(p => p.MobileSendings)
                    .HasForeignKey(d => d.Idpost)
                    .HasConstraintName("FK_MobileSendings_Posts");
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
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
