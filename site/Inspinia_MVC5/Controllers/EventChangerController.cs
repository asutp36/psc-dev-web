﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class EventChangerController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Region> _regions = null;
        List<Wash> _washes = null;
        List<Changer> _changers = null;
        //List<Device> _devices = null;
        List<EventChangerKind> _eventKinds = null;

        public EventChangerController()
        {
            _washes = db.Washes.Where(w => w.Code == "М13" || w.Code == "М14").ToList();
            //_devices = db.Devices.ToList();

            _regions = new List<Region>();
            _changers = new List<Changer>();

            _eventKinds = db.EventChangerKinds.Where(e => e.Code == "exchange" || e.Code == "cardCreate" || e.Code == "cardIncrease").ToList();

            foreach (Wash w in _washes)
            {
                if (!_regions.Contains(w.Region))
                    _regions.Add(w.Region);

                _changers.Add(db.Changers.ToList().Find(c => c.IDWash == w.IDWash));
            }

            foreach (var r in _regions)
            {
                for (int i = r.Washes.Count - 1; i >= 0; i--)
                {
                    string code = r.Washes.ElementAt(i).Code;

                    if (code == "М13" || code == "М14")
                    {
                    }
                    else
                    {
                        r.Washes.Remove(r.Washes.ElementAt(i));
                    }
                }
            }

            ViewBag.Regions = _regions;
            ViewBag.Washes = _washes;
            ViewBag.Changers = _changers;
            ViewBag.Events = _eventKinds;
        }

        public ActionResult EventChangerView(string begdate, string enddate, string changer)
        {
            //Post Post = _posts.Find(w => w.Code == post);

            //ViewBag.Region = Post.Wash.Region.Code;
            //ViewBag.Wash = Post.Wash.Code;
            //ViewBag.Post = Post.Code;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = DateTime.Today.AddYears(-10);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            ViewBag.BegDate = begdate;
            ViewBag.EndDate = enddate;

            return View("EventChangerView");
        }
        //public ActionResult _EventChangerList(string changer, string begdate, string enddate, string operation)
        //{
        //    List<GetEventChanger_Result> view = GetEventChanger(changer, begdate, enddate, operation);

        //    return PartialView("_EventChangerList", view);
        //}

        //public List<GetEventChanger_Result> GetEventChanger(string changer, string begdate, string enddate, string operation)
        //{
        //    List<GetEventChanger_Result> resultlist = null;

        //    DateTime bdate;
        //    if (!DateTime.TryParse(begdate, out bdate))
        //        bdate = DateTime.Today.AddYears(-10);

        //    DateTime edate;
        //    if (!DateTime.TryParse(enddate, out edate))
        //        edate = DateTime.Now;

        //    var prmChanger = new System.Data.SqlClient.SqlParameter("@p_ChangerID", System.Data.SqlDbType.NVarChar);
        //    if (changer == null || changer == "")
        //    {
        //        changer = "none";
        //    }
        //    prmChanger.Value = changer;

        //    var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
        //    prmBegDate.Value = bdate;

        //    var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
        //    prmEndDate.Value = edate;

        //    var prmOperation = new System.Data.SqlClient.SqlParameter("@p_EventChangerKind", System.Data.SqlDbType.NVarChar);
        //    if (operation == null)
        //    {
        //        operation = "";
        //    }
        //    prmOperation.Value = operatsql


        //    var result = db.Database.SqlQuery<GetEventChanger_Result>
        //        ("GetEventChanger @p_DateBeg, @p_DateEnd, @p_ChangerID, @p_EventChangerKind ",
        //        prmBegDate, prmEndDate, prmChanger, prmOperation).ToList();

        //    resultlist = result;

        //    return resultlist;
        //}

        //public ActionResult EventChangerFilter(string changer, string begdate, string enddate, string operation)
        //{
        //    List<GetEventChanger_Result> view = GetEventChanger(changer, begdate, enddate, operation);

        //    return PartialView("_EventChangerList", view);
        //}
    }
}