using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PostSyncService.Controllers.BindingModels;
using PostSyncService.Models.GateWash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostSyncService.Controllers.Helpers
{
    public class GateWashSqlHelper
    {
        private readonly GateWashDbContext _model;
        public GateWashSqlHelper()
        {
            _model = new GateWashDbContext();
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
                Event e = new Event
                {
                    Idsession = this.GetIdSession(eincr.cardNum, eincr.uuid),
                    IdeventOnPost = eincr.idEventPost,
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
                ei.IdeventNavigation = e;

                await _model.EventIncrease.AddAsync(ei);

                await _model.SaveChangesAsync();

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

        public async Task<int> WriteEventPayoutAsync(EventPayoutBindingModel epayout)
        {
            try
            {
                Event e = new Event
                {
                    Idsession = this.GetIdSession(epayout.cardNum, epayout.uuid),
                    IdeventOnPost = epayout.idEventPost,
                    Iddevice = this.GetIdDevice(epayout.deviceCode),
                    IdeventKind = this.GetIdEventKind(epayout.eventKindCode),
                    Dtime = DateTime.Parse(epayout.dtime)
                };

                EventPayout ep = new EventPayout
                {
                    Amount = epayout.amount,
                    B50 = epayout.b50,
                    B100 = epayout.b100,
                    StorageB50 = epayout.storage_b50,
                    StorageB100 = epayout.storage_b100
                };
                ep.IdeventNavigation = e;

                await _model.EventPayout.AddAsync(ep);

                await _model.SaveChangesAsync();

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
    }
}
