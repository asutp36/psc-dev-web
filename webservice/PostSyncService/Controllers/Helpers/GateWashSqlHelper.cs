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

        public async Task<string> WriteCardAsync(CardBindingModel card)
        {
            Cards c = new Cards() { Idcard = card.idCard, Iddevice = _model.Device.Where(d => d.Code == card.deviceCode).FirstOrDefault().Iddevice };
            await _model.Cards.AddAsync(c);
            await _model.SaveChangesAsync();

            return c.Idcard;
        }

        public bool IsCardExsists(string id)
        {
            return _model.Cards.Where(c => c.Idcard == id).FirstOrDefault() != null;
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
                Iddevice = this.GetIdDevice(session.deviceCode),
                Idfunction = this.GetIdFunction(session.functionCode),
                Idcard = session.idCard,
                Dtime = DateTime.Parse(session.dtime),
                Uuid = session.uuid
            };

            await _model.Sessions.AddAsync(s);
            await _model.SaveChangesAsync();

            return s.Idsession;
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

        public bool IsSessionExsists(string idCard, string uuid)
        {
            return _model.Sessions.Where(s => s.Idcard == idCard && s.Uuid == uuid).FirstOrDefault() != null;
        }

        public async Task<int> WriteEventAsync(EventBindingModel evnt)
        {
            Event e = new Event()
            {
                Idsession = this.GetIdSession(evnt.idCard, evnt.uuid),
                Iddevice = this.GetIdDevice(evnt.deviceCode),
                IdeventKind = this.GetIdEventKind(evnt.eventKindCode),
                Dtime = DateTime.Parse(evnt.dtime)
            };

            await _model.Event.AddAsync(e);
            await _model.SaveChangesAsync();

            return e.Idevent;
        }

        public int GetIdSession(string idCard, string uuid)
        {
            return _model.Sessions.Where(s => s.Idcard == idCard && s.Uuid == uuid).FirstOrDefault().Idsession;
        }

        public int GetIdEventKind(string code)
        {
            return _model.EventKind.Where(ek => ek.Code == code).FirstOrDefault().IdeventKind;
        }

        public bool IsEventKindExsists(string code)
        {
            return _model.EventKind.Where(ek => ek.Code == code).FirstOrDefault() != null;
        }
    }
}
