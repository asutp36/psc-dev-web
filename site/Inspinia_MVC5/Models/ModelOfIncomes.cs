namespace Inspinia_MVC5.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelOfIncomes : DbContext
    {
        public ModelOfIncomes()
            : base("name=ModelOfIncomes")
        {
        }

        public virtual DbSet<Income> Incomes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Income>()
                .Property(e => e.Region)
                .IsUnicode(false);

            modelBuilder.Entity<Income>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<Income>()
                .Property(e => e.Incomes)
                .IsUnicode(false);
        }
    }
}
