using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class WashLoyalityDbContext : DbContext
    {
        public WashLoyalityDbContext()
        {
        }

        public WashLoyalityDbContext(DbContextOptions<WashLoyalityDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<EachNwashCondition> EachNwashConditions { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<HappyHourCondition> HappyHourConditions { get; set; }
        public virtual DbSet<HolidayCondition> HolidayConditions { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<PromotionType> PromotionTypes { get; set; }
        public virtual DbSet<Terminal> Terminals { get; set; }
        public virtual DbSet<VipCondition> VipConditions { get; set; }
        public virtual DbSet<Wash> Washes { get; set; }
        public virtual DbSet<Washing> Washings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=WashLoyality;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Idclient);

                entity.Property(e => e.Idclient).HasColumnName("IDClient");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.Iddevice);

                entity.ToTable("Device");

                entity.Property(e => e.Iddevice).HasColumnName("IDDevice");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.IpAddress).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.ServerId).HasColumnName("ServerID");
            });

            modelBuilder.Entity<EachNwashCondition>(entity =>
            {
                entity.HasKey(e => e.Idpromotion);

                entity.ToTable("EachNWashConditions");

                entity.Property(e => e.Idpromotion)
                    .ValueGeneratedNever()
                    .HasColumnName("IDPromotion");

                entity.HasOne(d => d.IdpromotionNavigation)
                    .WithOne(p => p.EachNwashCondition)
                    .HasForeignKey<EachNwashCondition>(d => d.Idpromotion)
                    .HasConstraintName("FK_EachNWashConditions_Promotions");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Idgroup)
                    .HasName("PK_WashGroups");

                entity.Property(e => e.Idgroup).HasColumnName("IDGroup");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<HappyHourCondition>(entity =>
            {
                entity.HasKey(e => e.Idpromotion)
                    .HasName("PK_HappyHourPromotions");

                entity.Property(e => e.Idpromotion)
                    .ValueGeneratedNever()
                    .HasColumnName("IDPromotion");

                entity.HasOne(d => d.IdpromotionNavigation)
                    .WithOne(p => p.HappyHourCondition)
                    .HasForeignKey<HappyHourCondition>(d => d.Idpromotion)
                    .HasConstraintName("FK_HappyHourConditions_Promotions");
            });

            modelBuilder.Entity<HolidayCondition>(entity =>
            {
                entity.HasKey(e => e.Idpromotion)
                    .HasName("PK_HolidayPromotions");

                entity.Property(e => e.Idpromotion)
                    .ValueGeneratedNever()
                    .HasColumnName("IDPromotion");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.HasOne(d => d.IdpromotionNavigation)
                    .WithOne(p => p.HolidayCondition)
                    .HasForeignKey<HolidayCondition>(d => d.Idpromotion)
                    .HasConstraintName("FK_HolidayConditions_Promotions");
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

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.HasKey(e => e.Idpromotion);

                entity.Property(e => e.Idpromotion).HasColumnName("IDPromotion");

                entity.Property(e => e.Idgroup).HasColumnName("IDGroup");

                entity.Property(e => e.IdpromotionType).HasColumnName("IDPromotionType");

                entity.HasOne(d => d.IdgroupNavigation)
                    .WithMany(p => p.Promotions)
                    .HasForeignKey(d => d.Idgroup)
                    .HasConstraintName("FK_Promotions_Groups");

                entity.HasOne(d => d.IdpromotionTypeNavigation)
                    .WithMany(p => p.Promotions)
                    .HasForeignKey(d => d.IdpromotionType)
                    .HasConstraintName("FK_Promotions_PromotionTypes");
            });

            modelBuilder.Entity<PromotionType>(entity =>
            {
                entity.HasKey(e => e.IdpromotionType);

                entity.Property(e => e.IdpromotionType).HasColumnName("IDPromotionType");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
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

            modelBuilder.Entity<VipCondition>(entity =>
            {
                entity.HasKey(e => e.Idpromotion)
                    .HasName("PK_VipPromotions");

                entity.Property(e => e.Idpromotion)
                    .ValueGeneratedNever()
                    .HasColumnName("IDPromotion");

                entity.HasOne(d => d.IdpromotionNavigation)
                    .WithOne(p => p.VipCondition)
                    .HasForeignKey<VipCondition>(d => d.Idpromotion)
                    .HasConstraintName("FK_VipConditions_Promotions");
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

                entity.Property(e => e.Idgroup).HasColumnName("IDGroup");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasOne(d => d.IdgroupNavigation)
                    .WithMany(p => p.Washes)
                    .HasForeignKey(d => d.Idgroup)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Wash_Groups");
            });

            modelBuilder.Entity<Washing>(entity =>
            {
                entity.HasKey(e => e.Idwashing)
                    .HasName("PK_Table_1_1");

                entity.Property(e => e.Idwashing).HasColumnName("IDWashing");

                entity.Property(e => e.Dtime)
                    .HasColumnType("datetime")
                    .HasColumnName("DTime");

                entity.Property(e => e.Idclient).HasColumnName("IDClient");

                entity.Property(e => e.Idterminal).HasColumnName("IDTerminal");

                entity.HasOne(d => d.IdclientNavigation)
                    .WithMany(p => p.Washings)
                    .HasForeignKey(d => d.Idclient)
                    .HasConstraintName("FK_Washings_Clients");

                entity.HasOne(d => d.IdterminalNavigation)
                    .WithMany(p => p.Washings)
                    .HasForeignKey(d => d.Idterminal)
                    .HasConstraintName("FK_Washings_Terminals");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
