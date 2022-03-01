using Microsoft.EntityFrameworkCore;
using PostBackgroundServices.Models.WashCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PostBackgroundServices.Helpers
{
    class SqlHelper
    {
        public static List<MobileSending> GetUnsentWastes(WashCompanyContext context)
        {
            return context.MobileSendings.Where(s => (HttpStatusCode)s.StatusCode != HttpStatusCode.OK).ToList();
        }

        public static async Task UpdateWaste(WashCompanyContext context, MobileSending waste)
        {
            context.MobileSendings.Update(waste);
            await context.SaveChangesAsync();
        }
    }
}
