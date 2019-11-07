using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class OperationsTypedWashController : Controller
    {
        private ModelDb db = new ModelDb();

        List<CardType> _cardTypes = null;
        List<Psce> _psces = null;
        List<CardStatus> _cardStatuses = null;

        public OperationsTypedWashController()
        {
            _cardTypes = db.CardTypes.ToList();
            _psces = db.Psces.ToList();
            _cardStatuses = db.CardStatuses.ToList();

            ViewBag.CardTypes = _cardTypes;
            ViewBag.Psces = _psces;
            ViewBag.CardStatuses = _cardStatuses;
        }

        public ActionResult OperationsCardsTypedWash(string begTime, string endTime)
        {
            DateTime startDTime;
            if (!DateTime.TryParse(begTime, out startDTime))
                startDTime = DateTime.Today.AddDays(-1);

            DateTime stopDTime;
            if (!DateTime.TryParse(endTime, out stopDTime))
                stopDTime = DateTime.Today.AddSeconds(-1);

            ViewBag.timeOperationBeg = startDTime.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.timeOperationEnd = stopDTime.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.cardNum = "";
            ViewBag.operation = "";
            ViewBag.psc = null;
            ViewBag.function = "";
            ViewBag.details = "";

            return View();
        }

        public ActionResult UpdateViewBagOperationsCardsTypedWash(string begTime, string endTime, string cardNum,
            string operation, string psc, string function, string details)
        {
            List<GetCardList_Result> viewList = GetOperationsCardsTypedWash(begTime,endTime,cardNum,operation,psc,function,details);

            return PartialView("_OperationsCardsTypedWashList", viewList);
        }

        private List<GetCardList_Result> GetOperationsCardsTypedWash(string begTime, string endTime,
            string cardNum, string operation, string psc, string function, string details)
        {
            List<GetCardList_Result> resultset = null;

            var prmCardNum = new System.Data.SqlClient.SqlParameter("@p_CardNum", System.Data.SqlDbType.NVarChar);
            prmCardNum.Value = cardNum;

            var prmCardTypeCode = new System.Data.SqlClient.SqlParameter("@p_CardTypeCode", System.Data.SqlDbType.NVarChar);
            prmCardTypeCode.Value = operation;

            var prmCardStatusName = new System.Data.SqlClient.SqlParameter("@p_CardStatusName", System.Data.SqlDbType.NVarChar);
            prmCardStatusName.Value = function;

            var prmActivationDateBeg = new System.Data.SqlClient.SqlParameter("@p_ActivationDateBeg", System.Data.SqlDbType.DateTime);
            if (begTime == "")
            {
                prmActivationDateBeg.Value = new DateTime(2000, 1, 1);
            }
            else
            {
                prmActivationDateBeg.Value = begTime;
            }

            var prmActivationDateEnd = new System.Data.SqlClient.SqlParameter("@p_ActivationDateEnd", System.Data.SqlDbType.DateTime);
            if (endTime == "")
            {
                prmActivationDateEnd.Value = DateTime.Now;
            }
            else
            {
                prmActivationDateEnd.Value = endTime;
            }

            var prmActivationBy = new System.Data.SqlClient.SqlParameter("@p_ActivationBy", System.Data.SqlDbType.Int);
            prmActivationBy.Value = Convert.ToInt32(psc);

            var prmPhone = new System.Data.SqlClient.SqlParameter("@p_Phone", System.Data.SqlDbType.NVarChar);
            prmPhone.Value = "";

            var prmBalanceMin = new System.Data.SqlClient.SqlParameter("@p_BalanceMin", System.Data.SqlDbType.Int);
            prmBalanceMin.Value = 0;

            var prmBalanceMax = new System.Data.SqlClient.SqlParameter("@p_BalanceMax", System.Data.SqlDbType.Int);
            prmBalanceMax.Value = 0;

            var prmLastOperationDateBeg = new System.Data.SqlClient.SqlParameter("@p_LastOperationDateBeg", System.Data.SqlDbType.DateTime);
            prmLastOperationDateBeg.Value = new DateTime(2000, 1, 1);

            var prmLastOperationDateEnd = new System.Data.SqlClient.SqlParameter("@p_LastOperationDateEnd", System.Data.SqlDbType.DateTime);
            prmLastOperationDateEnd.Value = DateTime.Now;

            var prmLastOperationBy = new System.Data.SqlClient.SqlParameter("@p_LastOperationBy", System.Data.SqlDbType.Int);
            prmLastOperationBy.Value = 0;

            var prmIncreaseSumMin = new System.Data.SqlClient.SqlParameter("@p_IncreaseSumMin", System.Data.SqlDbType.Int);
            prmIncreaseSumMin.Value = 0;

            var prmIncreaseSumMax = new System.Data.SqlClient.SqlParameter("@p_IncreaseSumMax", System.Data.SqlDbType.Int);
            prmIncreaseSumMax.Value = 0;

            var prmDecreaseSumMin = new System.Data.SqlClient.SqlParameter("@p_DecreaseSumMin", System.Data.SqlDbType.Int);
            prmDecreaseSumMin.Value = 0;

            var prmDecreaseSumMax = new System.Data.SqlClient.SqlParameter("@p_DecreaseSumMax", System.Data.SqlDbType.Int);
            prmDecreaseSumMax.Value = 0;

            var prmCountOperationMin = new System.Data.SqlClient.SqlParameter("@p_CountOperationMin", System.Data.SqlDbType.Int);
            prmCountOperationMin.Value = 0;

            var prmCountOperationMax = new System.Data.SqlClient.SqlParameter("@p_CountOperationMax", System.Data.SqlDbType.Int);
            prmCountOperationMax.Value = 0;

            var result = db.Database
                .SqlQuery<GetCardList_Result>(
                    "GetCardList @p_Phone, @p_CardNum, @p_CardTypeCode, @p_CardStatusName, @p_BalanceMin," +
                    "@p_BalanceMax, @p_ActivationDateBeg, @p_ActivationDateEnd, @p_ActivationBy, @p_LastOperationDateBeg, " +
                    "@p_LastOperationDateEnd, @p_LastOperationBy, @p_IncreaseSumMin, @p_IncreaseSumMax, @p_DecreaseSumMin, " +
                    "@p_DecreaseSumMax, @p_CountOperationMin, @p_CountOperationMax",
                    prmPhone, prmCardNum, prmCardTypeCode, prmCardStatusName, prmBalanceMin, prmBalanceMax, prmActivationDateBeg,
                    prmActivationDateEnd, prmActivationBy, prmLastOperationDateBeg, prmLastOperationDateEnd, prmLastOperationBy,
                    prmIncreaseSumMin, prmIncreaseSumMax, prmDecreaseSumMin, prmDecreaseSumMax, prmCountOperationMin, prmCountOperationMax).ToList();

            resultset = result;

            return resultset;
        }

        // GET: OperationsCardsTypedWash
        public ActionResult Index()
        {
            return View();
        }
    }
}