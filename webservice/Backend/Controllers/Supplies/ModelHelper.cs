using Backend.Controllers.Supplies.Auth;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies
{
    public class ModelHelper
    {
        public static void WriteUser(AccountRequestModel model)
        {
            ModelDbContext context = new ModelDbContext();

            if (context.Database.CanConnect())
            {
                context.Database.OpenConnection();
                try
                {
                    context.Database.BeginTransaction();

                    string command = $"insert into Users (Login, Password, Email, Description, IDRole) " +
                        $"values ('{model.login}', '{model.password}', '{model.email}', '{model.description}', " +
                        $"(select IDRole from Roles where Code = '{model.role}'))";
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

                    throw new Exception("command");
                }
            }
            else
                throw new Exception("connection");
        }
    }
}
