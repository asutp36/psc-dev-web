using Backend.Controllers.Supplies.Stored_Procedures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public partial class ModelDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GetIncreaseByEvents_Result>().HasNoKey();
            modelBuilder.Entity<GetIncreaseByWashs_Result>().HasNoKey();
            modelBuilder.Entity<GetIncreaseByPosts_Result>().HasNoKey();
            modelBuilder.Entity<GetCollectByWashs_Result>().HasNoKey();
            modelBuilder.Entity<GetCollectByPosts_Result>().HasNoKey();
            modelBuilder.Entity<GetCollectByDays_Result>().HasNoKey();
            modelBuilder.Entity<GetBoxByWashs_Result>().HasNoKey();
            modelBuilder.Entity<GetBoxByPosts_Result>().HasNoKey();
            modelBuilder.Entity<GetSumsByChanger_Result>().HasNoKey();
        }
    }
}
