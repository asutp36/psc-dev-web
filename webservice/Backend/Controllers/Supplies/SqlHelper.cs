using Backend.Controllers.Supplies.Auth;
using Backend.Models;
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
    }
}
