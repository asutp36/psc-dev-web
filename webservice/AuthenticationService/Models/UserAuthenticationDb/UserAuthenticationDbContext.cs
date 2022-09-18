using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AuthenticationService.Models.UserAuthenticationDb
{
    public partial class UserAuthenticationDbContext : DbContext
    {
        public UserAuthenticationDbContext()
        {
        }

        public UserAuthenticationDbContext(DbContextOptions<UserAuthenticationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserWash> UserWashes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=UserAuthentication;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

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

                entity.HasIndex(e => new { e.Iduser, e.WashCode }, "UC_UserWash")
                    .IsUnique();

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.Property(e => e.WashCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasOne(d => d.IduserNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Iduser)
                    .HasConstraintName("FK_UserWash_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
