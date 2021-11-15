using Backend.Controllers.Supplies.Auth;
using Backend.Controllers.Supplies.Stored_Procedures;
using Backend.Controllers.Supplies.ViewModels;
using Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies
{
    public class SqlHelper
    {
        public static void WriteUser(AccountRequestModel model)
        {
            PhoneFormatter formattedPhone = new PhoneFormatter(model.phone);

            string email = "'null'";
            if (model.email != null)
                email = $"'{model.email}'";

            ModelDbContext context = new ModelDbContext();

            if (context.Database.CanConnect())
            {
                context.Database.OpenConnection();
                try
                {
                    context.Database.BeginTransaction();
                    string command = "";

                    command = $"insert into Users (Login, Password, Name, Email, Phone, PhoneInt, IDRole) " +
                        $"values ('{model.login}', '{model.password}', '{model.name}', {email}, {formattedPhone.phone}, " +
                        $"{formattedPhone.phoneInt}, (select IDRole from Roles where Code = '{model.role}'))";
                    context.Database.ExecuteSqlRaw(command);

                    foreach (string w in model.washes)
                    {
                        command = $"insert into UserWash (IDUser, IDWash) values " +
                            $"((select IDUser from Users where Login = '{model.login}'), " +
                            $"(select IDWash from Wash where Code = '{w}'))";
                        context.Database.ExecuteSqlRaw(command);
                    }

                    context.Database.CommitTransaction();
                    context.Database.CloseConnection();
                }
                catch (Exception e)
                {
                    context.Database.RollbackTransaction();

                    if (context.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                        context.Database.CloseConnection();

                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }

        public static void DeleteUser(string login)
        {
            ModelDbContext context = new ModelDbContext();

            if (context.Database.CanConnect())
            {
                context.Database.OpenConnection();
                try
                {
                    string command = $"delete from Users where Login = '{login}'";
                    context.Database.ExecuteSqlRaw(command);
                    context.Database.CloseConnection();
                }
                catch (Exception e)
                {
                    if (context.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                        context.Database.CloseConnection();

                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }

        public static void UpdateUser(UpdateAccountRequestModel model)
        {
            PhoneFormatter formattedPhone = new PhoneFormatter(model.phone);

            string email = "'null'";
            if (model.email != null)
                email = $"'{model.email}'";

            ModelDbContext context = new ModelDbContext();

            if (context.Database.CanConnect())
            {
                context.Database.OpenConnection();
                try
                {
                    context.Database.BeginTransaction();

                    string command = $"update Users set Login = '{model.login}', Name = '{model.name}', " +
                        $"Email = {email}, Phone = {formattedPhone.phone}, PhoneInt = {formattedPhone.phoneInt}, " +
                        $"IDRole = (select IDRole from Roles where Code = '{model.role}') where Login = '{model.oldLogin}';";
                    context.Database.ExecuteSqlRaw(command);

                    command = $"delete from UserWash where IDUser = (select IDUser from Users where Login = '{model.login}');";
                    context.Database.ExecuteSqlRaw(command);

                    foreach (string w in model.washes)
                    {
                        command = $"insert into UserWash (IDUser, IDWash) values " +
                            $"((select IDUser from Users where Login = '{model.login}'), " +
                            $"(select IDWash from Wash where Code = '{w}'))";
                        context.Database.ExecuteSqlRaw(command);
                    }

                    context.Database.CommitTransaction();
                    context.Database.CloseConnection();
                }
                catch (Exception e)
                {
                    context.Database.RollbackTransaction();

                    if (context.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                        context.Database.CloseConnection();

                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }

        public static List<GetBoxByPosts_Result> GetBoxByPosts(string reportDate, int regionCode, string washCode, string postCode)
        {
            ModelDbContext context = new ModelDbContext();

            if (context.Database.CanConnect())
            {
                try
                {
                    SqlParameter p_ReportDate = new SqlParameter("@p_ReportDate", reportDate);
                    SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                    SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                    SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                    List<GetBoxByPosts_Result> result = context.Set<GetBoxByPosts_Result>().FromSqlRaw("GetBoxByPosts @p_ReportDate, @p_RegionCode, @p_WashCode, @p_PostCode", p_ReportDate, p_RegionCode, p_WashCode, p_PostCode).ToList();
                    return result;
                }
                catch(Exception e)
                {
                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }

        public static GetBoxByWashs_Result GetBoxByWashs(string reportDate, int regionCode, string washCode)
        {
            ModelDbContext context = new ModelDbContext();

            if (context.Database.CanConnect())
            {
                try
                {
                    SqlParameter p_ReportDate = new SqlParameter("@p_ReportDate", reportDate);
                    SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                    SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);

                    GetBoxByWashs_Result result = context.Set<GetBoxByWashs_Result>().FromSqlRaw("GetBoxByWashs @p_ReportDate, @p_RegionCode, @p_WashCode", p_ReportDate, p_RegionCode, p_WashCode).AsEnumerable().FirstOrDefault();

                    return result;
                }
                catch (Exception e)
                {
                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }

        public static List<GetIncreaseByPosts_Result> GetIncreaseByPosts(string startDate, string endDate, int regionCode, string washCode, string postCode)
        {
            ModelDbContext context = new ModelDbContext();

            if (context.Database.CanConnect())
            {
                try
                {
                    SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                    SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                    SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                    SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                    SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                    List<GetIncreaseByPosts_Result> result = context.Set<GetIncreaseByPosts_Result>().FromSqlRaw("GetIncreaseByPosts @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode, p_PostCode).ToList();
                    return result;
                }
                catch (Exception e)
                {
                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }

        public static List<GetIncreaseByWashs_Result> GetIncreaseByWashs(string startDate, string endDate, int regionCode, string washCode)
        {
            ModelDbContext context = new ModelDbContext();

            if (context.Database.CanConnect())
            {
                try
                {
                    SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                    SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                    SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                    SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);

                    List<GetIncreaseByWashs_Result> result = context.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashs @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode).ToList();
                    return result;
                }
                catch (Exception e)
                {
                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }

        public static List<GetIncreaseByWashs_Result> GetIncreaseByWashsAfterLastCollect(int regionCode, string washCode)
        {
            ModelDbContext context = new ModelDbContext();

            if (context.Database.CanConnect())
            {
                try
                {
                    SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                    SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);

                    List<GetIncreaseByWashs_Result> result = context.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashsAfterLastCollect @p_RegionCode, @p_WashCode", p_RegionCode, p_WashCode).ToList();
                    return result;
                }
                catch (Exception e)
                {
                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }

        public static List<GetCollectByDays_Result> GetCollectByDays(string startDate, string endDate, int regionCode, string washCode)
        {
            ModelDbContext context = new ModelDbContext();

            if (context.Database.CanConnect())
            {
                try
                {
                    SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                    SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                    SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                    SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);

                    List<GetCollectByDays_Result> result = context.Set<GetCollectByDays_Result>().FromSqlRaw("GetCollectByDays @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode).ToList();
                    return result;
                }
                catch (Exception e)
                {
                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }

        public static DateTime GetLastPostSync(string postCode)
        {
            ModelDbContext context = new ModelDbContext();

            if (context.Database.CanConnect()) 
            {
                try
                {
                    DateTime result = context.Event.Include(e => e.IdpostNavigation).ThenInclude(p => p.IddeviceNavigation)
                                       .Where(res => res.IdpostNavigation.IddeviceNavigation.Code == postCode).Max(e => e.Dtime);

                    return result;
                }
                catch(Exception e)
                {
                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");

            
        }

        public static string GetWashCode(string postCode)
        {
            ModelDbContext context = new ModelDbContext();

            return context.Posts.Include(p => p.IdwashNavigation)
                                .Include(p => p.IddeviceNavigation)
                                .Where(d => d.IddeviceNavigation.Code == postCode).FirstOrDefault().IdwashNavigation.Code;
        }

        public static List<WashViewModel> GetWashesByRegion(int regionCode)
        {
            try
            {
                using (ModelDbContext context = new ModelDbContext())
                {
                    List<WashViewModel> result = context.Wash.Include(w => w.IdregionNavigation)
                                                             .Where(r => r.IdregionNavigation.Code == regionCode)
                                                             .Select(w => new WashViewModel { 
                                                                  idWash = w.Idwash,
                                                                  code = w.Code,
                                                                  name = w.Name,
                                                                  idRegion = w.Idregion
                                                              }).ToList();
                    return result;
                }
            }
            catch(Exception e)
            {
                throw new Exception("command", e);
            }
        }

        public static bool IsWashExists(string wash)
        {
            using (ModelDbContext context = new ModelDbContext())
            {
                Wash w = context.Wash.Where(w => w.Code == wash).FirstOrDefault();

                return w != null;
            }
        }

        public static RegionViewModel GetRegionByPost(string post)
        {
            using (ModelDbContext context = new ModelDbContext())
            {
                RegionViewModel region = context.Posts.Include(p => p.IddeviceNavigation).Where(d => d.IddeviceNavigation.Code == post)
                                                      .Include(p => p.IdwashNavigation).ThenInclude(w => w.IdregionNavigation)
                                                      .Select(r => new RegionViewModel
                                                      {
                                                          code = r.IdwashNavigation.IdregionNavigation.Code,
                                                          name = r.IdwashNavigation.IdregionNavigation.Name
                                                      }).FirstOrDefault();

                return region;
            }
        }

        public static RegionViewModel GetRegionByWash(string wash)
        {
            using (ModelDbContext context = new ModelDbContext())
            {
                RegionViewModel region = context.Wash.Where(w => w.Code == wash).Include(w => w.IdregionNavigation)
                                                      .Select(r => new RegionViewModel
                                                      {
                                                          code = r.IdregionNavigation.Code,
                                                          name = r.IdregionNavigation.Name
                                                      }).FirstOrDefault();

                return region;
            }
        }

        public static WashViewModel GetWashByPost(string post)
        {
            using (ModelDbContext context = new ModelDbContext())
            {
                WashViewModel wash = context.Posts.Include(p => p.IddeviceNavigation).Where(d => d.IddeviceNavigation.Code == post)
                                                  .Include(p => p.IdwashNavigation).Select(w => new WashViewModel
                                                  {
                                                      code = w.IdwashNavigation.Code,
                                                      name = w.IdwashNavigation.Name
                                                  }).FirstOrDefault();
                return wash;
            }
        }

        public static WashViewModel GetWashByCode(string code)
        {
            using (ModelDbContext context = new ModelDbContext())
            {
                WashViewModel wash = context.Wash.Where(w => w.Code == code).Select(w => new WashViewModel
                {
                    idWash = w.Idwash,
                    code = w.Code,
                    name = w.Name,
                    idRegion = w.Idregion
                }).FirstOrDefault();

                return wash;
            }
        }

        public static List<CardViewModel> GetTechCardsByWash(string wash)
        {
            using ModelDbContext context = new ModelDbContext();
            List<CardViewModel> result = context.Wash.Where(w => w.Code == wash)
                                                     .Join(context.WashGroup.Include(wg => wg.IdgroupNavigation),
                                                           w => w.Idwash,
                                                           wg => wg.Idwash,
                                                           (w, wg) => wg.IdgroupNavigation)
                                                     .Join(context.CardGroup.Include(cg => cg.IdcardNavigation).ThenInclude(c => c.IdcardTypeNavigation),
                                                           g => g.Idgroup,
                                                           cg => cg.Idgroup,
                                                           (g, cg) => new CardViewModel { num = cg.IdcardNavigation.CardNum, type = cg.IdcardNavigation.IdcardTypeNavigation.Name })
                                                     .OrderBy(c => c.type).OrderBy(c => c.num).ToList();
            return result;
        }

        public static List<GroupViewModel> GetGroupsTechCardsByWash(string wash)
        {
            using ModelDbContext context = new ModelDbContext();
            List<GroupViewModel> result = context.Wash.Where(w => w.Code == wash)
                                                      .Join(context.WashGroup.Include(wg => wg.IdgroupNavigation),
                                                            w => w.Idwash,
                                                            wg => wg.Idwash,
                                                            (w, wg) => new GroupViewModel { idGroup = wg.IdgroupNavigation.Idgroup, code = wg.IdgroupNavigation.Code, name = wg.IdgroupNavigation.Name })
                                                      .ToList();

            foreach(GroupViewModel group in result)
            {
                group.cards = context.Groups.Where(g => g.Idgroup == group.idGroup)
                                                    .Join(context.CardGroup.Include(cg => cg.IdcardNavigation).ThenInclude(c => c.IdcardTypeNavigation),
                                                          g => g.Idgroup,
                                                          cg => cg.Idgroup,
                                                          (g, cg) => new CardViewModel { num = cg.IdcardNavigation.CardNum, type = cg.IdcardNavigation.IdcardTypeNavigation.Name })
                                                    .OrderBy(c => c.type).ThenBy(c => c.num)
                                                    .ToList();
            }

            return result;                                          
        }

        public static List<CardTypeViewModel> GetTechCardTypes()
        {
            using ModelDbContext context = new ModelDbContext();
            return context.CardTypes.Where(t => t.Code != "client").Select(ct => new CardTypeViewModel { idCardType = ct.IdcardType, code = ct.Code, name = ct.Name }).ToList();
        }

        public static void WriteTechCard(TechCardModel model)
        {
            using ModelDbContext context = new ModelDbContext();
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Database.ExecuteSqlRaw($"insert into Cards(IDOwner, CardNum, IDCardStatus, IDCardType, LocalizedBy, LocalizedID) " +
                        $"values((select IDOwner from Owners where PhoneInt = 0), '{model.num}', (select IDCardStatus from CardStatuses where Code = 'norm'), " +
                        $"(select IDCardType from CardTypes where Code = '{model.typeCode}'), 0, 0)");

                    foreach(string groupCode in model.groupCodes)
                        context.Database.ExecuteSqlRaw($"insert into CardGroup(IDCard, IDGroup) values ((select IDCard from Cards where CardNum = '{model.num}'), (select IDGroup from Groups where Code = '{groupCode}'))");

                    context.SaveChanges();
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("command", e);
                }
            }
        }

        public static bool IsCardExists(string cardNum)
        {
            using ModelDbContext context = new ModelDbContext();
            return context.Cards.Where(c => c.CardNum == cardNum).FirstOrDefault() != null;
        }

        public static void DeleteCard(string cardNum)
        {
            using ModelDbContext context = new ModelDbContext();
            context.Database.ExecuteSqlRaw($"delete from Cards where CardNum = '{cardNum}'");
        }
    }
}
