using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PostSynchronizationService.Controllers.Models;
using PostSynchronizationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostSynchronizationService.Controllers.Helpers
{
    public class SqlHelper
    {
        public static async Task<int> WriteEventIncreaseAsync(ModelDbContext context, EventIncreaseModel model)
        {
            try
            {
                Event e = new()
                {
                    Idpost = GetIdPost(context, model.device),
                    IdeventKind = GetIdEventKind(context, model.eventKindCode),
                    Dtime = model.dtime,
                    IdeventPost = model.idEventPost
                };

                EventIncrease ei = new()
                {
                    IdeventNavigation = e,
                    Amount = model.amount,
                    M10 = model.m10,
                    B10 = model.b10,
                    B50 = model.b50,
                    B100 = model.b100,
                    B200 = model.b200,
                    Balance = model.balance,
                    IdpostSession = model.idPostSession
                };

                await context.EventIncreases.AddAsync(ei);
                context.SaveChangesAsync();

                return e.Idevent;
            }
            catch (SqlException e)
            {
                throw new Exception("command", e);
            }
            catch (DbUpdateException e)
            {
                if (e.HResult == -2146232060) // проблема с внешними ключами
                {
                    throw new Exception("command", e);
                }

                throw new Exception("db", e);
            }
        }

        public static async Task<int> WriteEventIncreaseWithSessionAsync(ModelDbContext context, EventIncreaseModel model)
        {
            try
            {
                Event e = new()
                {
                    Idpost = GetIdPost(context, model.device),
                    IdeventKind = GetIdEventKind(context, model.eventKindCode),
                    Dtime = model.dtime,
                    IdeventPost = model.idEventPost
                };

                EventIncrease ei = new()
                {
                    IdeventNavigation = e,
                    Amount = model.amount,
                    M10 = model.m10,
                    B10 = model.b10,
                    B50 = model.b50,
                    B100 = model.b100,
                    B200 = model.b200,
                    Balance = model.balance,
                    IdpostSession = GetIdPostSession(context, model.idPostSession, model.device)
                };

                await context.EventIncreases.AddAsync(ei);
                context.SaveChangesAsync();

                return e.Idevent;
            }
            catch (SqlException e)
            {
                throw new Exception("command", e);
            }
            catch (DbUpdateException e)
            {
                if (e.HResult == -2146232060) // проблема с внешними ключами
                {
                    throw new Exception("command", e);
                }

                throw new Exception("db", e);
            }
        }

        private static int GetIdPost(ModelDbContext context, string deviceCode)
        {
            return context.Posts.Where(p => p.IddeviceNavigation.Code == deviceCode).FirstOrDefault().Idpost;
        }

        private static int GetIdEventKind(ModelDbContext context, string eventKindCode)
        {
            return context.EventKinds.First(ek => ek.Code == eventKindCode).IdeventKind;
        }

        public static bool IsDeviceExists(ModelDbContext context, string code)
        {
            return context.Devices.Where(d => d.Code == code).FirstOrDefault() != null;
        }

        public static bool IsEventKindExists(ModelDbContext context, string code)
        {
            return context.EventKinds.Where(ek => ek.Code == code).FirstOrDefault() != null;
        }

        public static bool IsPostSessionExists(ModelDbContext context, int idSession, string deviceCode)
        {
            int idPost = GetIdPost(context, deviceCode);
            return context.PostSessions.Where(ps => ps.IdsessionOnPost == idSession && ps.Idpost == idPost ).FirstOrDefault() != null;
        }

        private static int GetIdPostSession(ModelDbContext context, int idSession, string deviceCode)
        {
            int idPost = GetIdPost(context, deviceCode);
            return context.PostSessions.Where(ps => ps.IdsessionOnPost == idSession && ps.Idpost == idPost).FirstOrDefault().IdpostSession;
        }
    }
}
