﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CashIntegration.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ModelDb : DbContext
    {
        public ModelDb()
            : base("name=ModelDb")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<EventIncrease> EventIncrease { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
    }
}
