using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class ChangerOperationsController : Controller
    {
        private ModelDb db = new ModelDb();
        List<Region> _regions = null;
        List<Device> _devices = null;
        List<Device> _requiredChangers = null;

        public ChangerOperationsController()
        {
            _devices = db.Devices.ToList();
            _regions = new List<Region>();
            _requiredChangers = new List<Device>();

            var washes = db.Washes.Where(w => w.Code == "М13" || w.Code == "М14").ToList();
            var changers = db.Changers.ToList();

            foreach (Wash w in washes)
            {
                if (!_regions.Contains(w.Region))
                    _regions.Add(w.Region);

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

            if (_requiredChangers.Count < 1)
            {
                foreach (var c in changers)
                {
                    var changer = _devices.Find(d => d.IDDevice == c.IDDevice);

                    _requiredChangers.Add(changer);
                }
            }
            else
            {
                var mobileapp = _devices.Find(d => d.Code == "MOB-EM");

                _requiredChangers.Add(mobileapp);
            }

            if(_regions.Count < 1)
            {
                _regions = db.Regions.ToList();
            }

            ViewBag.Regions = _regions;
            ViewBag.Changers = _requiredChangers;
        }

        public ActionResult ChangerOperationsView(string begdate, string enddate)
        {
            ViewBag.BegDate = begdate;
            ViewBag.EndDate = enddate;

            return View("ChangerOperationsView");
        }

        public ActionResult _ChangerOperationsList(string region, string changer, string begdate, string enddate)
        {
            List<GetSumsByChanger_Result> view = GetSumsByChanger(region, changer, begdate, enddate);

            return PartialView("_ChangerOperationsList", view);
        }

        public List<GetSumsByChanger_Result> GetSumsByChanger(string region, string changer, string begdate, string enddate)
        {
            List<GetSumsByChanger_Result> resultlist = null;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = new DateTime(2019, 1, 1);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            var prmRegion = new System.Data.SqlClient.SqlParameter("@p_RegionCode", System.Data.SqlDbType.Int);
            if (region == "")
            {
                region = "0";
            }
            prmRegion.Value = Convert.ToInt32(region);

            var prmChanger = new System.Data.SqlClient.SqlParameter("@p_ChangerCode", System.Data.SqlDbType.NVarChar);
            if (changer == null)
            {
                changer = "";
            }
            prmChanger.Value = changer;

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var result = db.Database.SqlQuery<GetSumsByChanger_Result>
                ("GetSumsByChanger @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_ChangerCode ",
                prmBegDate, prmEndDate, prmRegion, prmChanger).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult ChangerOperationsFilter(string region, string changer, string begdate, string enddate)
        {
            List<GetSumsByChanger_Result> view = GetSumsByChanger(region, changer, begdate, enddate);

            return PartialView("_ChangerOperationsList", view);
        }
    }
}