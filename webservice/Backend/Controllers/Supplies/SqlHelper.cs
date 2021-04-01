using Backend.Controllers.Supplies.Auth;
using Backend.Controllers.Supplies.Stored_Procedures;
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

        public static GetBoxByPosts_Result GetBoxByPosts(string reportDate, int regionCode, string washCode, string postCode)
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

                    GetBoxByPosts_Result result = context.Set<GetBoxByPosts_Result>().FromSqlRaw("GetBoxByPosts @p_ReportDate, @p_RegionCode, @p_WashCode, @p_PostCode", p_ReportDate, p_RegionCode, p_WashCode, p_PostCode).AsEnumerable().FirstOrDefault();
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

        public static GetIncreaseByPosts_Result GetIncreaseByPosts(string startDate, string endDate, int regionCode, string washCode, string postCode)
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

                    GetIncreaseByPosts_Result result = context.Set<GetIncreaseByPosts_Result>().FromSqlRaw("GetIncreaseByPosts @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode, p_PostCode).AsEnumerable().FirstOrDefault();
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
    }
}
