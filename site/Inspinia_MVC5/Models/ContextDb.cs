using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Inspinia_MVC5.Models
{
    public class ContextDb : DbContext
    {
        public ContextDb() : base("ModelDb")
        { }
        public DbSet<Card> Cards { get; set; }
        public DbSet<CardStatus> CardStatuses { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Psce> Psces { get; set; }
    }
}