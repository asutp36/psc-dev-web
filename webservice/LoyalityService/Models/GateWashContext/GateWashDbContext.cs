using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DiscountService.Models.GateWashContext
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

        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Washing> Washings { get; set; }

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
            });

            modelBuilder.Entity<Washing>(entity =>
            {
                entity.HasKey(e => e.Idwashing)
                    .HasName("PK_Washing");

                entity.Property(e => e.Idwashing).HasColumnName("IDWashing");

                entity.Property(e => e.Dtime)
                    .HasColumnType("datetime")
                    .HasColumnName("DTime");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.HasOne(d => d.IddeviceNavigation)
                    .WithMany(p => p.Washings)
                    .HasForeignKey(d => d.Iddevice)
                    .HasConstraintName("FK_Washings_Device");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
