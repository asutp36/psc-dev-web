using GateWashDataService.Models.GateWashContext.StoredProcedures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models.GateWashContext
{
    public partial class GateWashDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<spGetCommulativeTotal_Result>().HasNoKey();
        }
    }
}
