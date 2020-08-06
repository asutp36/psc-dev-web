using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;
using Inspinia_MVC5.ViewModels;

namespace Inspinia_MVC5.Controllers
{
    public class EventChangerController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Device> _devices = null;
        List<Device> _requiredChangers = null;
        List<EventChangerKind> _eventKinds = null;

        public EventChangerController()
        {
            _devices = db.Devices.ToList();
            _eventKinds = db.EventChangerKinds.Where(e => e.Code == "exchange" || e.Code == "cardCreate" || e.Code == "cardIncrease").ToList();

            _requiredChangers = new List<Device>();

            var washes = db.Washes.Where(w => w.Code == "М13" || w.Code == "М14").ToList();
            var changers = db.Changers.ToList();

            foreach (Wash w in washes)
            {
                var chs = changers.FindAll(c => c.IDWash == w.IDWash);

                foreach (var c in chs)
                {
                    var device = _devices.Find(d => d.IDDevice == c.IDDevice);

                    if (device != null)
                    {
                        _requiredChangers.Add(device);
                    }
                }
            }

            ViewBag.Changers = _requiredChangers;
            ViewBag.Events = _eventKinds;
        }

        public ActionResult EventChangerView(string begdate, string enddate)
        {
            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = new DateTime(2019, 1, 1);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            ViewBag.BegDate = begdate;
            ViewBag.EndDate = enddate;

            return View("EventChangerView");
        }

        public ActionResult _EventChangerList(string begdate, string enddate, string changerCode, string kindEventCode)
        {
            List<EventChangerWithDetails> view = GetEventChanger(begdate, enddate, changerCode, kindEventCode);

            return PartialView("_EventChangerList", view);
        }

        public List<EventChangerWithDetails> GetEventChanger(string begdate, string enddate, string changerCode, string kindEventCode)
        {
            List<EventChangerWithDetails> resultlist = new List<EventChangerWithDetails>();

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = new DateTime(2019, 1, 1);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var prmChangerCode = new System.Data.SqlClient.SqlParameter("@p_ChangerCode", System.Data.SqlDbType.NVarChar);
            if (changerCode == null)
            {
                changerCode = "";
            }
            prmChangerCode.Value = changerCode;

            var prmKindEventCode = new System.Data.SqlClient.SqlParameter("@p_KindEventCode", System.Data.SqlDbType.NVarChar);
            if (kindEventCode == null)
            {
                kindEventCode = "";
            }
            prmKindEventCode.Value = kindEventCode;

            var result = db.Database.SqlQuery<GetEventsByChanger_Result>
                ("GetEventsByChanger @p_DateBeg, @p_DateEnd, @p_ChangerCode, @p_KindEventCode ",
                prmBegDate, prmEndDate, prmChangerCode, prmKindEventCode).ToList();

            foreach(var r in result)
            {
                List<GetDataEventsByChanger_Result> details = GetDetailsEventChanger(begdate, enddate, r.IDEventChanger);

                var e = new EventChangerWithDetails(r.ChangerName, r.DTime, r.KindEvent, details);

                resultlist.Add(e);
            }

            return resultlist;
        }

        public ActionResult EventChangerFilter(string begdate, string enddate, string changerCode, string kindEventCode)
        {
            List<EventChangerWithDetails> view = GetEventChanger(begdate, enddate, changerCode, kindEventCode);

            return PartialView("_EventChangerList", view);
        }

        public List<GetDataEventsByChanger_Result> GetDetailsEventChanger(string begdate, string enddate, int idEventChanger)
        {
            List<GetDataEventsByChanger_Result> resultlist = null;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = new DateTime(2019, 1, 1);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var prmChangerCode = new System.Data.SqlClient.SqlParameter("@p_ChangerCode", System.Data.SqlDbType.NVarChar);
            prmChangerCode.Value = "";

            var prmKindEventCode = new System.Data.SqlClient.SqlParameter("@p_KindEventCode", System.Data.SqlDbType.NVarChar);
            prmKindEventCode.Value = "";

            var prmIDEventChanger = new System.Data.SqlClient.SqlParameter("@IDEventChanger", System.Data.SqlDbType.Int);
            prmIDEventChanger.Value = idEventChanger;

            var result = db.Database.SqlQuery<GetDataEventsByChanger_Result>
                ("GetDataEventsByChanger @p_DateBeg, @p_DateEnd, @p_ChangerCode, @p_KindEventCode, @IDEventChanger ",
                prmBegDate, prmEndDate, prmChangerCode, prmKindEventCode, prmIDEventChanger).ToList();

            resultlist = result;

            return resultlist;
        }
    }
}