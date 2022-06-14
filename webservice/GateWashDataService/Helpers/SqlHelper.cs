using GateWashDataService.Models;
using GateWashDataService.Models.Filters;
using GateWashDataService.Models.GateWashContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Helpers
{
    public class SqlHelper
    {
        public static async Task<List<RegionModel>> GetRegions(GateWashDbContext context)
        {
            return context.Regions.Select(r => new RegionModel
            {
                IdRegion = r.Idregion,
                Code = r.Code,
                Name = r.Name
            }).ToList();
        }

        public static List<CollectModel> GetCollects(GateWashDbContext context, GetCollectsParameters param)
        {
            return context.Collects.Where(c => (c.Dtime >= param.StartDate && c.Dtime <= param.EndDate)
                                               && (param.Terminal == null || c.IddeviceNavigation.Code == param.Terminal))
                                   .Select(c => new CollectModel
                                   {
                                       DTime = c.Dtime,
                                       Terminal = c.IddeviceNavigation.Code,
                                       m10 = c.M10,
                                       b50 = c.B50,
                                       b100 = c.B100,
                                       b200 = c.B200,
                                       b500 = c.B500,
                                       b1000 = c.B1000,
                                       b2000 = c.B2000
                                   }).ToList();
        }

        public static void WriteUser(AccountRequestModel model, GateWashDbContext context)
        {
            PhoneFormatter formattedPhone = new PhoneFormatter(model.phone);

            string email = "'null'";
            if (model.email != null)
                email = $"'{model.email}'";

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

        public static void DeleteUser(string login, GateWashDbContext context)
        {
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

        public static void UpdateUser(UpdateAccountRequestModel model, GateWashDbContext context)
        {
            PhoneFormatter formattedPhone = new PhoneFormatter(model.phone);

            string email = "'null'";
            if (model.email != null)
                email = $"'{model.email}'";

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
    }
}
