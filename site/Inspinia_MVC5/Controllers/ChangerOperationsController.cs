using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;
using Inspinia_MVC5.Controllers;
using System.Net;
using Newtonsoft.Json;
using Inspinia_MVC5.Helpers;
using System.Globalization;

namespace Inspinia_MVC5.Controllers
{
    public class ChangerOperationsController : Controller
    {
        private ModelDb db = new ModelDb();
        List<Region> _regions = null;
        List<Device> _devices = null;
        List<Device> _requiredChangers = null;

        MonitoringController monitoringController = new MonitoringController();

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

            var rs = db.Regions.ToList().Find(r => r.Code == 200);

            _regions.Add(rs);

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

                var mobileapp = _devices.Find(d => d.Code == "MOB-EM");

                _requiredChangers.Remove(mobileapp);
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
            List<ChangerSumData> view = new List<ChangerSumData>();

            List<GetSumsByChanger_Result> resFromProcedure = GetSumsByChanger(region, changer, begdate, enddate);

            foreach(var r in resFromProcedure)
            {
                Device ch = _devices.Find(d => d.Code == r.ChangerCode);

                var response = monitoringController.GetInfoChanger(ch);

                ChangerSumData changerSumData;

                var nf = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                nf.NumberGroupSeparator = " ";

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    InfoChanger infoChanger = JsonConvert.DeserializeObject<InfoChanger>(response.Result);
                    infoChanger.changer = ch;

                    int boxIncrease =
                        infoChanger.m10 * 10 +
                        infoChanger.b50 * 50 +
                        infoChanger.b100 * 100 +
                        infoChanger.b200 * 200 +
                        infoChanger.b500 * 500 +
                        infoChanger.b1000 * 1000 +
                        infoChanger.b2000 * 2000;

                    int boxOut =
                        infoChanger.box1_50 * 50 +
                        infoChanger.box2_100 * 100 +
                        infoChanger.box3_50 * 50 +
                        infoChanger.box4_100 * 100;

                    if(infoChanger.hopper !=null)
                    {
                        boxOut += infoChanger.box5_10 * 10;
                    }

                    changerSumData = new ChangerSumData(
                        ch.Code,
                        ch.Name,
                        ((int)r.sincrease).ToString("0,0.00"),
                        ((int)r.sout).ToString("0,0.00"),
                        ((int)r.ccard).ToString(nf),
                        boxIncrease.ToString("0,0.00"),
                        boxOut.ToString("0,0.00"),
                        infoChanger.availableCards.ToString(nf)
                        );

                    view.Add(changerSumData);
                }
                else
                {
                    changerSumData = new ChangerSumData(
                        ch.Code,
                        ch.Name,
                        ((int)r.sincrease).ToString("0,0.00"),
                        ((int)r.sout).ToString("0,0.00"),
                        ((int)r.ccard).ToString(nf),
                        "нет доступа",
                        "нет доступа",
                        "нет доступа"
                        );

                    view.Add(changerSumData);
                }
            }

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