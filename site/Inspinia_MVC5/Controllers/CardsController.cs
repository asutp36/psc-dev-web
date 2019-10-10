using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class CardsController : Controller
    {
        private ModelDb db = new ModelDb();

        List<CardType> _cardTypes = null;
        List<CardStatus> _cardStatuses = null;
        List<Psce> _psces = null;

        public CardsController()
        {
            _cardTypes = db.CardTypes.ToList();
            _cardStatuses = db.CardStatuses.ToList();
            _psces = db.Psces.ToList();

            ViewBag.CardTypes = _cardTypes;
            ViewBag.CardStatuses = _cardStatuses;
            ViewBag.Psces = _psces;
        }

        //LO - LastOperation
        public ActionResult Cards(string begTimeLO, string endTimeLO)
        {
            DateTime startDTimeLO;
            if (!DateTime.TryParse(begTimeLO, out startDTimeLO))
                startDTimeLO = DateTime.Today.AddDays(-1);

            DateTime stopDTimeLO;
            if (!DateTime.TryParse(endTimeLO, out stopDTimeLO))
                stopDTimeLO = DateTime.Today.AddSeconds(-1);

            ViewBag.cardNum = "";
            ViewBag.cardTypeCode = "";
            ViewBag.cardStatusName = "";
            ViewBag.balance = "";
            ViewBag.startDateActivation = "";
            ViewBag.stopDateActivasion = "";
            ViewBag.placeActivation = "";
            ViewBag.startDateLO = startDTimeLO.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.stopDateLO = stopDTimeLO.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.placeLO = "";
            ViewBag.sumIncashes = "";
            ViewBag.sumChardgeOff = "";
            ViewBag.amountOperations = "";

            return View();
        }

        public ActionResult UpdateViewBagCards(string cardNum, string cardType, string cardStatus,
            string balance, string startDateActivation, string endDateActivation, string placeActivation,
            string startDateLO, string endDateLO, string placeLO, string sumIncashes, string sumChardgeOff, string amountOperations)
        {
            //List<GetCards_Result> viewList = GetCardsFromDB(cardNum, cardType, cardStatus, balance, startDateActivation,
            //endDateActivation, placeActivation, startDateLO, endDateLO, placeLO, sumIncashes, sumChardgeOff, amountOperations);

            //return PartialView("_CardsList", viewList);
            return null;
        }

        //private List<GetCards_Result> GetCardsFromDB(string cardNum, string cardType, string cardStatus,
        //    string balance, string startDateActivation, string endDateActivation, string placeActivation,
        //    string startDateLO, string endDateLO, string placeLO, string sumIncashes, string sumChardgeOff, string amountOperations)
        //{
        //    List<GetCards_Result> resultset = null;

        //    DateTime startDTimeLO;
        //    if (!DateTime.TryParse(startDateLO, out startDTimeLO))
        //        startDTimeLO = DateTime.Today.AddDays(-1);

        //    DateTime stopDTimeLO;
        //    if (!DateTime.TryParse(endDateLO, out stopDTimeLO))
        //        stopDTimeLO = DateTime.Today.AddSeconds(-1);

        //    var prmCardNum = new System.Data.SqlClient.SqlParameter("@p_CardNum", System.Data.SqlDbType.NVarChar);
        //    prmCardNum.Value = cardNum;

        //    var prmCardTypeCode = new System.Data.SqlClient.SqlParameter("@p_CardTypeCode", System.Data.SqlDbType.NVarChar);
        //    prmCardTypeCode.Value = cardType;

        //    var prmCardStatusName = new System.Data.SqlClient.SqlParameter("@p_CardStatusName", System.Data.SqlDbType.NVarChar);
        //    prmCardStatusName.Value = cardStatus;

        //    var prmBalance = new System.Data.SqlClient.SqlParameter("@p_Balance", System.Data.SqlDbType.Int);
        //    prmBalance.Value = balance;

        //    var prmStartDateActivation = new System.Data.SqlClient.SqlParameter("@p_ActivationCardDateBeg", System.Data.SqlDbType.DateTime);
        //    prmStartDateActivation.Value = startDateActivation;

        //    var prmEndDateActivation= new System.Data.SqlClient.SqlParameter("@p_ActivationCardDateEnd", System.Data.SqlDbType.DateTime);
        //    prmEndDateActivation.Value = endDateActivation;

        //    var prmPlaceActivation = new System.Data.SqlClient.SqlParameter("@p_PlaceActivationCard", System.Data.SqlDbType.NVarChar);
        //    prmPlaceActivation.Value = placeActivation;

        //    var prmStartDateLO= new System.Data.SqlClient.SqlParameter("@p_LastOperationDateBeg", System.Data.SqlDbType.DateTime);
        //    prmStartDateLO.Value = startDateLO;

        //    var prmEndDateLO= new System.Data.SqlClient.SqlParameter("@p_LastOperationDateEnd", System.Data.SqlDbType.DateTime);
        //    prmEndDateLO.Value = endDateLO;

        //    var prmPlaceLO = new System.Data.SqlClient.SqlParameter("@p_PlaceLastOperation", System.Data.SqlDbType.NVarChar);
        //    prmPlaceLO.Value = placeLO;

        //    var prmSumIncahes = new System.Data.SqlClient.SqlParameter("@p_SumIncahes", System.Data.SqlDbType.Int);
        //    prmSumIncahes.Value = Convert.ToInt32(sumIncashes);

        //    var prmSumChardgeOff = new System.Data.SqlClient.SqlParameter("@p_SumChardgeOff", System.Data.SqlDbType.Int);
        //    prmSumChardgeOff.Value = Convert.ToInt32(sumChardgeOff);

        //    var prmAmountOperations = new System.Data.SqlClient.SqlParameter("@p_AmountOperations", System.Data.SqlDbType.Int);
        //    prmAmountOperations.Value = Convert.ToInt32(amountOperations);

        //    var result = db.Database
        //        .SqlQuery<GetCards_Result>("GetCards @p_CardNum, @p_CardTypeCode, @p_CardStatusName, @p_Balance," +
        //            "@p_ActivationCardDateBeg, @p_ActivationCardDateEnd, @p_PlaceActivationCard, @p_LastOperationDateBeg, @p_LastOperationDateEnd" +
        //            "@p_PlaceLastOperation, @p_SumIncashes, @p_SumChardgeOff, @p_AmountOperations", prmCardNum,
        //            prmCardTypeCode, prmCardStatusName, prmBalance, prmStartDateActivation, prmEndDateActivation, prmPlaceActivation,
        //            prmStartDateLO, prmEndDateLO, prmPlaceLO, prmPlaceLO, prmSumIncahes, prmSumChardgeOff, prmAmountOperations)
        //        .ToList();

        //    resultset = result;

        //    return resultset;
        //}
        // GET: Cards
        public ActionResult Index()
        {
            return View();
        }
    }
}