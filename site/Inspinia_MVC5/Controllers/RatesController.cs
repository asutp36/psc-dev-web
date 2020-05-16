using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using Inspinia_MVC5.Helpers;
using Inspinia_MVC5.Models;
using Newtonsoft.Json;

namespace Inspinia_MVC5.Controllers
{
    public class RatesController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Region> _regions = null;

        public RatesController()
        {
            _regions = db.Regions.ToList();
        }

        public ActionResult RatesView()
        {
            ViewBag.Regions = _regions;

            return View();
        }

        public ActionResult _RatesWashesList(string region)
        {
            List<Wash> _washes = null;

            if (region != null && region != "0")
            {
                int code = Convert.ToInt32(region);
                _washes = db.Regions.ToList().Find(r => r.Code == code).Washes.ToList();
            }

            return PartialView("_RatesWashesList", _washes);
        }

        public ActionResult _RateTableWash(string region)
        {
            List<Wash> _washes = null;

            if (region != null && region != "0" && region !="")
            {
                int code = Convert.ToInt32(region);
                _washes = db.Regions.ToList().Find(r => r.Code == code).Washes.ToList();
            }

            ViewBag.Washes = _washes;

            List<string> codes = new List<string>();

            foreach (var item in _washes)
            {
                codes.Add(item.Code);
            }

            string rates = GetRates(JsonConvert.SerializeObject(codes));

            //string rates = "[{\"Wash\":\"М 8\",\"Rates\":[{\"Post\":\"М8-1\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-2\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-3\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-4\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-5\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-6\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-7\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-8\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]}]},{\"Wash\":\"М 9\",\"Rates\":[{\"Post\":\"М9-1\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М9-2\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М9-3\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М9-4\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М9-5\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М9-6\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]}]},{\"Wash\":\"М14\",\"Rates\":[{\"Post\":\"М14-1\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-2\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-3\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-4\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-5\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-6\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-7\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-8\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V1\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V2\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V3\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V4\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V5\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V6\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V7\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V8\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V9\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]}]}]";
            rates = JsonConvert.DeserializeObject<string>(rates);

            List<RatesWash> _rates = new List<RatesWash>();

            _rates = JsonConvert.DeserializeObject<List<RatesWash>>(rates);

            return PartialView("_RateTableWash", _rates);
        }

        public ActionResult SaveNewRates(List<List<string>> rates, string wash)
        {
            string data = JsonConvert.SerializeObject(new ChangeRatesData(new List<string> {wash}, rates));
            string testlog = SendRates(data);

            List<Wash> _washes = null;

            _washes = db.Washes.ToList().Find(w => w.Code == wash).Region.Washes.ToList();

            ViewBag.Washes = _washes;

            List<string> codes = new List<string>();

            foreach (var item in _washes)
            {
                codes.Add(item.Code);
            }

            string ratess = GetRates(JsonConvert.SerializeObject(codes));

            //string rates = "[{\"Wash\":\"М 8\",\"Rates\":[{\"Post\":\"М8-1\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-2\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-3\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-4\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-5\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-6\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-7\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М8-8\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]}]},{\"Wash\":\"М 9\",\"Rates\":[{\"Post\":\"М9-1\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М9-2\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М9-3\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М9-4\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М9-5\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М9-6\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]}]},{\"Wash\":\"М14\",\"Rates\":[{\"Post\":\"М14-1\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-2\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-3\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-4\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-5\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-6\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-7\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-8\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V1\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V2\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V3\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V4\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V5\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V6\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V7\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V8\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]},{\"Post\":\"М14-V9\",\"Prices\":[{\"Name\":\"Предварительная\",\"Rate\":24},{\"Name\":\"Интенсив\",\"Rate\":23},{\"Name\":\"Пена\",\"Rate\":50},{\"Name\":\"Вода\",\"Rate\":18},{\"Name\":\"Теплая вода\",\"Rate\":23},{\"Name\":\"Воск\",\"Rate\":39},{\"Name\":\"Осмос\",\"Rate\":29},{\"Name\":\"Пауза\",\"Rate\":2}]}]}]";
            ratess = JsonConvert.DeserializeObject<string>(ratess);

            List<RatesWash> _rates = new List<RatesWash>();

            _rates = JsonConvert.DeserializeObject<List<RatesWash>>(ratess);

            return PartialView("_RateTableWash", _rates);
        }

        public string SendRates(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://194.87.98.177/postrc/api/post/rate");

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ptsv2.com/t/oxkpf-1589648006/post");

            //request.Host = "api.myeco24.ru";
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

            byte[] postBytes = Encoding.UTF8.GetBytes(json);

            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.ContentLength = postBytes.Length;

            Stream requestStream = request.GetRequestStream();

            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return response.ToString();
                }
                else
                {
                    string result;
                    using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                    {
                        result = rdr.ReadToEnd();
                    }

                    return String.Format("httpStatusCode: {0}; {1}", response.StatusCode, result);
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse webResponse = (HttpWebResponse)ex.Response;

                string result;
                using (StreamReader rdr = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }
                return result + "\nStatusCode: " + webResponse.StatusCode;
            }
        }

        public string GetRates(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://194.87.98.177/postrc/api/post/getrate");

            //request.Host = "api.myeco24.ru";
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

            byte[] postBytes = Encoding.UTF8.GetBytes(json);

            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.ContentLength = postBytes.Length;

            Stream requestStream = request.GetRequestStream();

            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return response.ToString();
                }
                else
                {
                    string result;
                    using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                    {
                        result = rdr.ReadToEnd();
                    }

                    return result;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse webResponse = (HttpWebResponse)ex.Response;

                string result;
                using (StreamReader rdr = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }
                return result + "\nStatusCode: " + webResponse.StatusCode;
            }
        }
    }
}