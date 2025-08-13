using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GateWashMobApp.Models.GateWashDb
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
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<Program> Program { get; set; }
        public virtual DbSet<Regions> Regions { get; set; }
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
