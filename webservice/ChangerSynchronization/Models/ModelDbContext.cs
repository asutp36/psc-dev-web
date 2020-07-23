using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ChangerSynchronization.Models
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
        public virtual DbSet<EventChanger> EventChanger { get; set; }
        public virtual DbSet<EventChangerAcquiring> EventChangerAcquiring { get; set; }
        public virtual DbSet<EventChangerCard> EventChangerCard { get; set; }
        public virtual DbSet<EventChangerIncrease> EventChangerIncrease { get; set; }
        public virtual DbSet<EventChangerKind> EventChangerKind { get; set; }
        public virtual DbSet<EventChangerOut> EventChangerOut { get; set; }
        public virtual DbSet<OperationTypes> OperationTypes { get; set; }
        public virtual DbSet<Operations> Operations { get; set; }
        public virtual DbSet<Owners> Owners { get; set; }
        public virtual DbSet<Wash> Wash { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=WashCompany;Trusted_Connection=True;");
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

                entity.Property(e => e.Idwash).HasColumnName("IDWash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.IdwashNavigation)
                    .WithMany(p => p.Changers)
                    .HasForeignKey(d => d.Idwash)
                    .HasConstraintName("FK_Changers_Wash");
            });

            modelBuilder.Entity<EventChanger>(entity =>
            {
                entity.HasKey(e => e.IdeventChanger);

                entity.Property(e => e.IdeventChanger).HasColumnName("IDEventChanger");

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Idchanger).HasColumnName("IDChanger");

                entity.Property(e => e.IdeventChangerKind).HasColumnName("IDEventChangerKind");

                entity.HasOne(d => d.IdchangerNavigation)
                    .WithMany(p => p.EventChanger)
                    .HasForeignKey(d => d.Idchanger)
                    .HasConstraintName("FK_EventChanger_Changers");

                entity.HasOne(d => d.IdeventChangerKindNavigation)
                    .WithMany(p => p.EventChanger)
                    .HasForeignKey(d => d.IdeventChangerKind)
                    .HasConstraintName("FK_EventChanger_EventChangerKind");
            });

            modelBuilder.Entity<EventChangerAcquiring>(entity =>
            {
                entity.HasKey(e => e.IdeventChangerAcquiring);

                entity.Property(e => e.IdeventChangerAcquiring).HasColumnName("IDEventChangerAcquiring");

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdeventChanger).HasColumnName("IDEventChanger");

                entity.HasOne(d => d.IdeventChangerNavigation)
                    .WithMany(p => p.EventChangerAcquiring)
                    .HasForeignKey(d => d.IdeventChanger)
                    .HasConstraintName("FK_EventChangerAcquiring_EventChanger");
            });

            modelBuilder.Entity<EventChangerCard>(entity =>
            {
                entity.HasKey(e => e.IdeventChangerCard);

                entity.Property(e => e.IdeventChangerCard).HasColumnName("IDEventChangerCard");

                entity.Property(e => e.CardNum)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdeventChanger).HasColumnName("IDEventChanger");

                entity.HasOne(d => d.IdeventChangerNavigation)
                    .WithMany(p => p.EventChangerCard)
                    .HasForeignKey(d => d.IdeventChanger)
                    .HasConstraintName("FK_EventChangerCard_EventChanger");
            });

            modelBuilder.Entity<EventChangerIncrease>(entity =>
            {
                entity.HasKey(e => e.IdeventChangerIncrease);

                entity.Property(e => e.IdeventChangerIncrease).HasColumnName("IDEventChangerIncrease");

                entity.Property(e => e.B100).HasColumnName("b100");

                entity.Property(e => e.B1000).HasColumnName("b1000");

                entity.Property(e => e.B200).HasColumnName("b200");

                entity.Property(e => e.B2000).HasColumnName("b2000");

                entity.Property(e => e.B50).HasColumnName("b50");

                entity.Property(e => e.B500).HasColumnName("b500");

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdeventChanger).HasColumnName("IDEventChanger");

                entity.Property(e => e.M10).HasColumnName("m10");

                entity.HasOne(d => d.IdeventChangerNavigation)
                    .WithMany(p => p.EventChangerIncrease)
                    .HasForeignKey(d => d.IdeventChanger)
                    .HasConstraintName("FK_EventChangerIncrease_EventChanger");
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

            modelBuilder.Entity<EventChangerOut>(entity =>
            {
                entity.HasKey(e => e.IdeventChangerOut);

                entity.Property(e => e.IdeventChangerOut).HasColumnName("IDEventChangerOut");

                entity.Property(e => e.B100).HasColumnName("b100");

                entity.Property(e => e.B50).HasColumnName("b50");

                entity.Property(e => e.Dtime)
                    .HasColumnName("DTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdeventChanger).HasColumnName("IDEventChanger");

                entity.Property(e => e.M10).HasColumnName("m10");

                entity.HasOne(d => d.IdeventChangerNavigation)
                    .WithMany(p => p.EventChangerOut)
                    .HasForeignKey(d => d.IdeventChanger)
                    .HasConstraintName("FK_EventChangerOut_EventChanger");
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

                entity.Property(e => e.Idchanger).HasColumnName("IDChanger");

                entity.Property(e => e.IdoperationType).HasColumnName("IDOperationType");

                entity.Property(e => e.LocalizedId).HasColumnName("LocalizedID");

                entity.HasOne(d => d.IdcardNavigation)
                    .WithMany(p => p.Operations)
                    .HasForeignKey(d => d.Idcard)
                    .HasConstraintName("FK_Operations_Cards");

                entity.HasOne(d => d.IdchangerNavigation)
                    .WithMany(p => p.Operations)
                    .HasForeignKey(d => d.Idchanger)
                    .HasConstraintName("FK_Operations_Changers");

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
