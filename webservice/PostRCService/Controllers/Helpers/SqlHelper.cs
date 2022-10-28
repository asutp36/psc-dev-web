using Microsoft.EntityFrameworkCore;
using PostRCService.Models;
using PostRCService.Models.WashCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.Helpers
{
    public class SqlHelper
    {
        public static bool IsWashExists(string washCode)
        {
            using(WashCompanyDbContext model = new WashCompanyDbContext())
            {
                var wash = model.Washes.Where(w => w.Code == washCode).FirstOrDefault();

                return wash != null;
            }
        }

        public static bool IsPostExists(string postCode)
        {
            using (WashCompanyDbContext model = new WashCompanyDbContext())
            {
                var post = model.Devices.Where(d => d.Code == postCode).FirstOrDefault();

                return post != null;
            }
        }

        public static string GetPostIp(string postCode)
        {
            using(WashCompanyDbContext model = new WashCompanyDbContext())
            {
                var device = model.Devices.Where(d => d.Code == postCode).FirstOrDefault();

                if (device == null)
                    return null;
                else
                    return device.IpAddress;
            }
        }

        public static List<string> GetPostCodes(string washCode)
        {
            using(WashCompanyDbContext model = new WashCompanyDbContext())
            {
                List<string> codes = model.Posts.Include(p => p.IddeviceNavigation).Include(p => p.IdwashNavigation).Where(w => w.IdwashNavigation.Code == washCode).Select(x => x.IddeviceNavigation.Code).ToList();

                return codes;
            }
        }
    }
}
