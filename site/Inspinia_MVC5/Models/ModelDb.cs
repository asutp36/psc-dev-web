namespace Inspinia_MVC5.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelDb : DbContext
    {
        public ModelDb()
            : base("name=ModelDb")
        {
        }

        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<CardStatus> CardStatuses { get; set; }
        public virtual DbSet<CardType> CardTypes { get; set; }
        public virtual DbSet<Operation> Operations { get; set; }
        public virtual DbSet<OperationType> OperationTypes { get; set; }
        public virtual DbSet<Owner> Owners { get; set; }
        public virtual DbSet<Psce> Psces { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>()
                .Property(e => e.CardNum)
                .IsUnicode(false);

            modelBuilder.Entity<CardStatus>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<CardStatus>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<CardType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<CardType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<OperationType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<OperationType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Owner>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Psce>()
                .Property(e => e.Name)
                .IsUnicode(false);
        }
    }
}
