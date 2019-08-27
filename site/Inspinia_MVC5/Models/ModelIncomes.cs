namespace Inspinia_MVC5.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelIncomes : DbContext
    {
        public ModelIncomes()
            : base("name=ModelIncomes")
        {
        }

        public virtual DbSet<Income> Incomes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
