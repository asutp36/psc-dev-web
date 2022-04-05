using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using GateWashSyncService.Controllers.BindingModels;
using GateWashSyncService.Models.GateWash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Controllers.Helpers
{
    public class GateWashSqlHelper
    {
        private readonly GateWashDbContext _model;
        public GateWashSqlHelper(GateWashDbContext model)
        {
            _model = model;
        }

        public async Task<int> WriteCardAsync(CardBindingModel card)
        {
            try
            {
                Cards c = new Cards() { CardNum = card.cardNum };
                await _model.Cards.AddAsync(c);
                await _model.SaveChangesAsync();

                return c.Idcard;
            }
            catch (SqlException e)
            {
                throw new Exception("command", e);
            }
            catch (DbUpdateException e)
            {
                throw new Exception("db", e);
            }
        }

        public bool IsCardExsists(string id)
        {
            return _model.Cards.Where(c => c.CardNum == id).FirstOrDefault() != null;
        }

        public bool IsDeviceExsists(string code)
        {
            return _model.Device.Where(d => d.Code == code).FirstOrDefault() != null;
        }

        public async Task<int> WriteSessionAsync(SessionBindingModel session)
        {
            try
            {
                Sessions s = new Sessions()
                {
                    IdsessoinOnWash = session.idSession,
                    Idfunction = this.GetIdFunction(session.functionCode),
                    Idcard = this.GetIdCard(session.cardNum),
                    Dtime = DateTime.Parse(session.dtime),
                    Uuid = session.uuid
                };

                await _model.Sessions.AddAsync(s);
                await _model.SaveChangesAsync();

                return s.Idsession;
            }
            catch (SqlException e)
            {
                throw new Exception("command", e);
            }
            catch (DbUpdateException e)
            {
                throw new Exception("db", e);
            }
        }

        public int GetIdCard(string cardNum)
        {
            return _model.Cards.Where(c => c.CardNum == cardNum).FirstOrDefault().Idcard;
        }

        public int GetIdDevice(string code)
        {
            return _model.Device.Where(d => d.Code == code).FirstOrDefault().Iddevice;
        }

        public int GetIdFunction(string code)
        {
            return _model.Functions.Where(f => f.Code == code).FirstOrDefault().Idfunction;
        }

        public bool IsFunctionExsists(string code)
        {
            return _model.Functions.Where(f => f.Code == code).FirstOrDefault() != null;
        }

        public bool IsSessionExsists(string cardNum, string uuid)
        {
            return _model.Sessions.Where(s => s.Idcard == _model.Cards.Where(c => c.CardNum == cardNum).FirstOrDefault().Idcard && s.Uuid == uuid).FirstOrDefault() != null;
        }

        public bool IsPaySessionExsists(string deviceCode, int idSessionOnPost)
        {
            return _model.PaySession.Include(s => s.IddeviceNavigation).Where(s => s.IddeviceNavigation.Code == deviceCode && s.IdsessionOnPost == idSessionOnPost).FirstOrDefault() != null;
        }

        public async Task<int> WriteEventAsync(EventBindingModel evnt)
        {
            try
            {
                Event e = new Event()
                {
                    Idsession = this.GetIdSession(evnt.cardNum, evnt.uuid),
                    IdeventOnPost = evnt.idEventOnPost,
                    Iddevice = this.GetIdDevice(evnt.deviceCode),
                    IdeventKind = this.GetIdEventKind(evnt.eventKindCode),
                    Dtime = DateTime.Parse(evnt.dtime)
                };

                await _model.Event.AddAsync(e);
                await _model.SaveChangesAsync();

                return e.Idevent;
            }
            catch(SqlException e)
            {
                throw new Exception("command", e);
            }
            catch(DbUpdateException e)
            {
                if (e.HResult == -2146232060) // проблема с внешними ключами
                {
                    throw new Exception("command", e);
                }

                throw new Exception("db", e);
            }
        }

        public int GetIdSession(string cardNum, string uuid)
        {
            return _model.Sessions.Where(s => s.Idcard == _model.Cards.Where(c => c.CardNum == cardNum).FirstOrDefault().Idcard && s.Uuid == uuid).FirstOrDefault().Idsession;
        }

        public int GetIdEventKind(string code)
        {
            return _model.EventKind.Where(ek => ek.Code == code).FirstOrDefault().IdeventKind;
        }

        public bool IsEventKindExsists(string code)
        {
            return _model.EventKind.Where(ek => ek.Code == code).FirstOrDefault() != null;
        }

        public async Task<int> WriteEventIncreaseAsync(EventIncreaseBindingModel eincr)
        {
            try
            {
                PayEvent pe = new PayEvent
                {
                    IdpaySession = this.GetIdPaySession(eincr.deviceCode, eincr.idSessionOnPost),
                    IdeventOnPost = eincr.idEventOnPost,
                    Iddevice = this.GetIdDevice(eincr.deviceCode),
                    IdeventKind = this.GetIdEventKind(eincr.eventKindCode),
                    Dtime = DateTime.Parse(eincr.dtime)
                };

                EventIncrease ei = new EventIncrease
                {
                    Amount = eincr.amount,
                    M10 = eincr.m10,
                    B50 = eincr.b50,
                    B100 = eincr.b100,
                    B200 = eincr.b200,
                    B500 = eincr.b500,
                    B1000 = eincr.b1000,
                    B2000 = eincr.b2000
                };
                ei.IdpayEventNavigation = pe;

                await _model.EventIncrease.AddAsync(ei);

                await _model.SaveChangesAsync();

                return pe.IdpayEvent;
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

        public async Task<int> WriteEventPayoutAsync(EventPayoutBindingModel epayout)
        {
            try
            {
                PayEvent e = new PayEvent
                {
                    IdpaySession = this.GetIdPaySession(epayout.deviceCode, epayout.idSessionOnPost),
                    IdeventOnPost = epayout.idEventOnPost,
                    Iddevice = this.GetIdDevice(epayout.deviceCode),
                    IdeventKind = this.GetIdEventKind(epayout.eventKindCode),
                    Dtime = DateTime.Parse(epayout.dtime)
                };

                EventPayout ep = new EventPayout
                {
                    Amount = epayout.amount,
                    B50 = epayout.b50,
                    B100 = epayout.b100,
                    Inbox1B50 = epayout.inbox_1_b50,
                    Inbox2B50 = epayout.inbox_2_b50,
                    Inbox3B100 = epayout.inbox_3_b100,
                    Inbox4B100 = epayout.inbox_4_b100
                };
                ep.IdpayEventNavigation = e;

                await _model.EventPayout.AddAsync(ep);

                await _model.SaveChangesAsync();

                return e.IdpayEvent;
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

        public async Task<int> WriteCollectAsync(CollectBindingModel collect)
        {
            try
            {
                Collect c = new Collect
                {
                    Iddevice = this.GetIdDevice(collect.deviceCode),
                    IdcollectOnPost = collect.idCollectOnPost,
                    Dtime = DateTime.Parse(collect.dtime),
                    Amount = collect.amount,
                    M10 = collect.m10,
                    B50 = collect.b50,
                    B100 = collect.b100,
                    B200 = collect.b200,
                    B500 = collect.b500,
                    B1000 = collect.b1000,
                    B2000 = collect.b2000,
                    BoxB50 = collect.box_b50,
                    BoxB100 = collect.box_b100,
                    InboxB50 = collect.inbox_b50,
                    InboxB100 = collect.inbox_b100
                };

                await _model.Collect.AddAsync(c);

                await _model.SaveChangesAsync();

                return c.Idcollect;
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

        public async Task<int> WritePaySessionAsync(PaySessionBindingModel psession)
        {
            try
            {
                PaySession ps = new PaySession()
                {
                    IdsessionOnPost = psession.idSessionOnPost,
                    Idfunction = this.GetIdFunction(psession.functionCode),
                    Dtime = DateTime.Parse(psession.dtime),
                    Iddevice = this.GetIdDevice(psession.deviceCode),
                    ProgramCost = psession.programCost,
                    Qr = psession.qr,
                    FiscalError = psession.fiscalError
                };

                await _model.PaySession.AddAsync(ps);

                await _model.SaveChangesAsync();

                return ps.IdpaySession;
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

        public int GetIdPaySession(string deviceCode, int idSessionOnPost)
        {
            return _model.PaySession.Include(s => s.IddeviceNavigation).Where(s => s.IdsessionOnPost == idSessionOnPost && s.IddeviceNavigation.Code == deviceCode).FirstOrDefault().IdpaySession;
        }
    }
}
