using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{

    public class OperationsController : Controller
    {
        private CardsDbEntities db = new CardsDbEntities();
        List<CardType> _cardTypes = null;
        List<CardStatus> _cardStatuses = null;
        List<OperationType> _operationTypes = null;

        List<object> testdb = new List<object>();

        public OperationsController()
        {
            _cardTypes = db.CardTypes.ToList();
            _cardStatuses = db.CardStatuses.ToList();
            _operationTypes = db.OperationTypes.ToList();
        }

        public ActionResult ProductsGrid()
        {
            return View();
        }

        public ActionResult OperationsCards()
        {
            DateTime startDTime = new DateTime(2019, 8, 1, 0, 0, 0);
            DateTime stopDTime = new DateTime(2019, 10, 21, 0, 0, 0);

            ViewBag.T_Operations = GetOperationsFromDB("","","","","",startDTime, stopDTime, 0,0);

            ViewBag.CardTypes = _cardTypes;
            ViewBag.CardStatuses = _cardStatuses;
            ViewBag.OperationTypes = _operationTypes;

            return View();
        }

        [HttpGet]
        [ActionName("UpdateViewBagOperations")]
        public ActionResult UpdateViewBagOperations(
            string phone, string cardNum, string cardTypeCode, string cardStatusName,
            string operationTypeName, string begTime, string endTime, int by, int id)
        {
            DateTime startDTime = new DateTime(2019, 8, 1, 0, 0, 0);
            DateTime stopDTime = new DateTime(2019, 10, 21, 0, 0, 0);

            ViewBag.T_Operations = GetOperationsFromDB(
                phone, cardNum, cardTypeCode, cardStatusName,
                operationTypeName, startDTime, stopDTime, by, id);

            return PartialView("~/Views/Dashboards/Update_Operation_Table.cshtml");
        }

        private List<GetCardsOperations_Result> GetOperationsFromDB(
            string phone, string cardNum, string cardTypeCode, string cardStatusName, 
            string operationTypeName, DateTime begTime, DateTime endTime, int by, int id)
        {
            List<GetCardsOperations_Result> resultset = null;

            var prmPhone = new System.Data.SqlClient.SqlParameter("@p_Phone", System.Data.SqlDbType.NVarChar);
            prmPhone.Value = phone;

            var prmCardNum = new System.Data.SqlClient.SqlParameter("@p_CardNum", System.Data.SqlDbType.NVarChar);
            prmCardNum.Value = cardNum;

            var prmCardTypeCode = new System.Data.SqlClient.SqlParameter("@p_CardTypeCode", System.Data.SqlDbType.NVarChar);
            prmCardTypeCode.Value = cardTypeCode;

            var prmCardStatusName = new System.Data.SqlClient.SqlParameter("@p_CardStatusName", System.Data.SqlDbType.NVarChar);
            prmCardStatusName.Value = cardStatusName;

            var prmOperationTypeName = new System.Data.SqlClient.SqlParameter("@p_OperationTypeName", System.Data.SqlDbType.NVarChar);
            prmOperationTypeName.Value = operationTypeName;

            var prmOperationDateBeg = new System.Data.SqlClient.SqlParameter("@p_OperationDateBeg", System.Data.SqlDbType.DateTime);
            prmOperationDateBeg.Value = begTime;

            var prmOperationDateEnd = new System.Data.SqlClient.SqlParameter("@p_OperationDateEnd", System.Data.SqlDbType.DateTime);
            prmOperationDateEnd.Value = endTime;

            var prmLocalizedBy = new System.Data.SqlClient.SqlParameter("@p_LocalizedBy", System.Data.SqlDbType.Int);
            prmLocalizedBy.Value = by;

            var prmLocalizedID = new System.Data.SqlClient.SqlParameter("@p_LocalizedID", System.Data.SqlDbType.Int);
            prmLocalizedID.Value = id;

            var result = db.Database
                .SqlQuery<GetCardsOperations_Result>("GetCardsOperations @p_Phone, @p_CardNum, @p_CardTypeCode, @p_CardStatusName," +
                    "@p_OperationTypeName, @p_OperationDateBeg, @p_OperationDateEnd, @p_LocalizedBy, @p_LocalizedID", prmPhone,
                    prmCardNum, prmCardTypeCode, prmCardStatusName, prmOperationTypeName, prmOperationDateBeg, prmOperationDateEnd,
                    prmLocalizedBy, prmLocalizedID)
                .ToList();

            resultset = result;

            return resultset;
        }
    }
}