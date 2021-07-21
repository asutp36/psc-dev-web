using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangFireTest.JobHelpers.Models
{
    public partial class WashCompanyDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GetDayReportIncrease_Result>().HasNoKey();
        }
    }
}
