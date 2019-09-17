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
        private ModelDb db = new ModelDb();

        List<CardType> _cardTypes = null;
        List<CardStatus> _cardStatuses = null;
        List<OperationType> _operationTypes = null;

        public OperationsController()
        {
            _cardTypes = db.CardTypes.ToList();
            _cardStatuses = db.CardStatuses.ToList();
            _operationTypes = db.OperationTypes.ToList();

            ViewBag.CardTypes = _cardTypes;
            ViewBag.CardStatuses = _cardStatuses;
            ViewBag.OperationTypes = _operationTypes;
        }

        public ActionResult OperationsCards(
            string begTime, string endTime)
        {
            DateTime startDTime;
            if (!DateTime.TryParse(begTime, out startDTime))
                startDTime = DateTime.Today.AddDays(-1);

            DateTime stopDTime;
            if (!DateTime.TryParse(endTime, out stopDTime))
                stopDTime = DateTime.Today.AddSeconds(-1);

            //ViewBag.T_Operations = GetOperationsFromDB("","","","","",startDTime, stopDTime, 0,0);

            //ViewBag.phone = phone;
            //ViewBag.cardNum = cardNum;
            //ViewBag.cardTypeCode = cardTypeCode;
            //ViewBag.cardStatusName = cardStatusName;
            //ViewBag.operationTypeName = operationTypeName;
            ViewBag.startDTime = startDTime.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.stopDTime = stopDTime.ToString("dd.MM.yyyy HH:mm:ss");
            //ViewBag.by = Convert.ToInt32(by);
            //ViewBag.id = Convert.ToInt32(id);

            return View();
        }

        //[HttpGet]
        //[ActionName("UpdateViewBagOperations")]
        public ActionResult UpdateViewBagOperations(
            string phone, string cardNum, string cardTypeCode, string cardStatusName,
            string operationTypeName, string begTime, string endTime, string by, string id)
        {
            //ViewBag.T_Operations = GetOperationsFromDB(phone, cardNum, cardTypeCode, cardStatusName,operationTypeName, begTime, endTime, by, id);

            List<GetCardsOperations_Result> viewList = GetOperationsFromDB(
                phone, cardNum, cardTypeCode, cardStatusName,
                operationTypeName, begTime, endTime, by, id);

            return PartialView("_OperationsCardsList", viewList);
        }

        private List<GetCardsOperations_Result> GetOperationsFromDB(
            string phone, string cardNum, string cardTypeCode, string cardStatusName, 
            string operationTypeName, string begTime, string endTime, string by, string id)
        {
            List<GetCardsOperations_Result> resultset = null;

            DateTime startDTime;
            if (!DateTime.TryParse(begTime, out startDTime))
                startDTime = DateTime.Today.AddDays(-1);

            DateTime stopDTime;
            if (!DateTime.TryParse(endTime, out stopDTime))
                stopDTime = DateTime.Today.AddSeconds(-1);

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
            prmOperationDateBeg.Value = startDTime;

            var prmOperationDateEnd = new System.Data.SqlClient.SqlParameter("@p_OperationDateEnd", System.Data.SqlDbType.DateTime);
            prmOperationDateEnd.Value = stopDTime;

            var prmLocalizedBy = new System.Data.SqlClient.SqlParameter("@p_LocalizedBy", System.Data.SqlDbType.Int);
            prmLocalizedBy.Value = Convert.ToInt32(by);

            var prmLocalizedID = new System.Data.SqlClient.SqlParameter("@p_LocalizedID", System.Data.SqlDbType.Int);
            prmLocalizedID.Value = Convert.ToInt32(id);

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