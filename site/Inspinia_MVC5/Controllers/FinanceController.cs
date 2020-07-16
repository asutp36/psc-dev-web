using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class FinanceController : Controller
    {
        private ModelDb db = new ModelDb();

        public FinanceController()
        {
        }

        public ActionResult FinanceOperations(string begTime, string endTime)
        {
            DateTime startDTime;
            if (!DateTime.TryParse(begTime, out startDTime))
                startDTime = DateTime.Today.AddDays(-1);

            DateTime stopDTime;
            if (!DateTime.TryParse(endTime, out stopDTime))
                stopDTime = DateTime.Today.AddSeconds(-1);

            ViewBag.timeOperationBeg = startDTime.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.timeOperationEnd = stopDTime.ToString("dd.MM.yyyy HH:mm:ss");

            return View();
        }

        public ActionResult UpdateViewBagFinanceOperations(string begTime, string endTime)
        {
            List<GetFinanceList_Result> viewList = GetFinanceOperations(begTime, endTime);

            return PartialView("_FinanceOperationsList", viewList);
        }

        private List<GetFinanceList_Result> GetFinanceOperations(string begTime, string endTime)
        {
            List<GetFinanceList_Result> resultset = null;

            var prmDateBeg = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            if (begTime == "")
            {
                prmDateBeg.Value = new DateTime(2000, 1, 1);
            }
            else
            {
                prmDateBeg.Value = begTime;
            }

            var prmDateEnd = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            if (endTime == "")
            {
                prmDateEnd.Value = DateTime.Now;
            }
            else
            {
                prmDateEnd.Value = endTime;
            }

            var result = db.Database
                .SqlQuery<GetFinanceList_Result>("GetFinanceList @p_DateBeg, @p_DateEnd", prmDateBeg, prmDateEnd).ToList();

            resultset = result;

            return resultset;
        }

        public ActionResult UpdateFinanceOperationsSummary(string begTime, string endTime)
        {
            Int32 summary = GetFinanceOperationsSummary(begTime, endTime);

            return PartialView("_FinanceOperationsSummary", summary);
        }

        private Int32 GetFinanceOperationsSummary(string begTime, string endTime)
        {
            Int32 summary = 0;

            var prmDateBeg = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            if (begTime == "")
            {
                prmDateBeg.Value = new DateTime(2000, 1, 1);
            }
            else
            {
                prmDateBeg.Value = begTime;
            }

            var prmDateEnd = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            if (endTime == "")
            {
                prmDateEnd.Value = DateTime.Now;
            }
            else
            {
                prmDateEnd.Value = endTime;
            }

            var result = db.Database
                .SqlQuery<GetFinanceSummary_Result>("GetFinanceSummary @p_DateBeg, @p_DateEnd", prmDateBeg, prmDateEnd).ToList();

            List<GetFinanceSummary_Result> resultset = result;

            if (resultset[0].s.HasValue)
                summary = resultset[0].s.Value;

            return summary;
        }

        public ActionResult UpdateFinanceOperationsByNominals(string begTime, string endTime)
        {
            int[] data = {0, 0, 0, 0, 0, 0, 0 };

            List<GetFinanceByNominals_Result> viewList = GetFinanceOperationsByNominals(begTime, endTime);

            foreach (GetFinanceByNominals_Result item in viewList)
            {
                switch(item.Code)
                {
                    case "M10":
                        data[0] = item.c.GetValueOrDefault();
                        break;
                    case "B50":
                        data[1] = item.c.GetValueOrDefault();
                        break;
                    case "B100":
                        data[2] = item.c.GetValueOrDefault();
                        break;
                    case "B200":
                        data[3] = item.c.GetValueOrDefault();
                        break;
                    case "B500":
                        data[4] = item.c.GetValueOrDefault();
                        break;
                    case "B1000":
                        data[5] = item.c.GetValueOrDefault();
                        break;
                    case "B2000":
                        data[6] = item.c.GetValueOrDefault();
                        break;
                }
            }

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            string chartdata = jsSerializer.Serialize(data);

            return PartialView("_FinanceOperationsByNominals", chartdata);
        }

        private List<GetFinanceByNominals_Result> GetFinanceOperationsByNominals(string begTime, string endTime)
        {
            List<GetFinanceByNominals_Result> resultset = null;

            var prmDateBeg = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            if (begTime == "")
            {
                prmDateBeg.Value = new DateTime(2000, 1, 1);
            }
            else
            {
                prmDateBeg.Value = begTime;
            }

            var prmDateEnd = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            if (endTime == "")
            {
                prmDateEnd.Value = DateTime.Now;
            }
            else
            {
                prmDateEnd.Value = endTime;
            }

            var result = db.Database
                .SqlQuery<GetFinanceByNominals_Result>("GetFinanceByNominals @p_DateBeg, @p_DateEnd", prmDateBeg, prmDateEnd).ToList();

            resultset = result;

            return resultset;
        }

        // GET: Finance
        public ActionResult Index()
        {
            return View();
        }
    }
}