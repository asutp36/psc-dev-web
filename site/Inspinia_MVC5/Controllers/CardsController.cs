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
        List<Changer> _changers = null;

        public CardsController()
        {
            _cardTypes = db.CardTypes.ToList();
            _cardStatuses = db.CardStatuses.ToList();
            _changers = db.Changers.ToList();

            ViewBag.CardTypes = _cardTypes;
            ViewBag.CardStatuses = _cardStatuses;
            ViewBag.Changers = _changers;
        }

        public ActionResult Cards(string begTimeLO, string endTimeLO, string begTimeActivation, string endTimeActivation)
        {
            DateTime startDTimeLO;
            if (!DateTime.TryParse(begTimeLO, out startDTimeLO))
                startDTimeLO = DateTime.Today.AddDays(-1);

            DateTime stopDTimeLO;
            if (!DateTime.TryParse(endTimeLO, out stopDTimeLO))
                stopDTimeLO = DateTime.Today.AddSeconds(-1);

            DateTime startDTimeActivation;
            if (!DateTime.TryParse(begTimeActivation, out startDTimeActivation))
                startDTimeActivation = DateTime.Today.AddMonths(-3);

            DateTime stopDTimeActivation;
            if (!DateTime.TryParse(endTimeActivation, out stopDTimeActivation))
                stopDTimeActivation = DateTime.Today.AddSeconds(-1);

            ViewBag.phone = "";
            ViewBag.cardNum = "";
            ViewBag.cardTypeCode = "";
            ViewBag.cardStatusName = "";
            ViewBag.balanceMin = null;
            ViewBag.balanceMax = null;
            ViewBag.activationDateBeg = startDTimeActivation.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.activationDateEnd = stopDTimeActivation.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.activationBy = null;
            ViewBag.lastOperationDateBeg = startDTimeLO.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.lastOperationDateEnd = stopDTimeLO.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.lastOperationBy = null;
            ViewBag.increaseSumMin = null;
            ViewBag.increaseSumMax = null;
            ViewBag.decreaseSumMin = null;
            ViewBag.decreaseSumMax = null;
            ViewBag.countOperationMin = null;
            ViewBag.countOperationMax = null;

            Diapasons();

            return View();
        }

        public ActionResult UpdateViewBagCards(
            string phone,
            string cardNum,
            string cardTypeCode,
            string cardStatusName,
            string balanceMin,
            string balanceMax,
            string activationDateBeg,
            string activationDateEnd,
            string activationBy,
            string lastOperationDateBeg,
            string lastOperationDateEnd,
            string lastOperationBy,
            string increaseSumMin,
            string increaseSumMax,
            string decreaseSumMin,
            string decreaseSumMax,
            string countOperationMin,
            string countOperationMax
            )
        {

            List<GetCardList_Result> viewList = GetCardsFromDB(phone, cardNum, cardTypeCode, cardStatusName, balanceMin,
            balanceMax, activationDateBeg, activationDateEnd, activationBy, lastOperationDateBeg, lastOperationDateEnd,
            lastOperationBy, increaseSumMin, increaseSumMax, decreaseSumMin, decreaseSumMax, countOperationMin, countOperationMax);

            return PartialView("_CardsList", viewList);
        }

        private List<GetCardList_Result> GetCardsFromDB(
            string phone,
            string cardNum,
            string cardTypeCode,
            string cardStatusName,
            string balanceMin,
            string balanceMax,
            string activationDateBeg,
            string activationDateEnd,
            string activationBy,
            string lastOperationDateBeg,
            string lastOperationDateEnd,
            string lastOperationBy,
            string increaseSumMin,
            string increaseSumMax,
            string decreaseSumMin,
            string decreaseSumMax,
            string countOperationMin,
            string countOperationMax
            )
        {
            List<GetCardList_Result> resultset = null;

            var prmPhone = new System.Data.SqlClient.SqlParameter("@p_Phone", System.Data.SqlDbType.NVarChar);
            prmPhone.Value = phone;

            var prmCardNum = new System.Data.SqlClient.SqlParameter("@p_CardNum", System.Data.SqlDbType.NVarChar);
            prmCardNum.Value = cardNum;

            var prmCardTypeCode = new System.Data.SqlClient.SqlParameter("@p_CardTypeCode", System.Data.SqlDbType.NVarChar);
            prmCardTypeCode.Value = cardTypeCode;

            var prmCardStatusName = new System.Data.SqlClient.SqlParameter("@p_CardStatusName", System.Data.SqlDbType.NVarChar);
            prmCardStatusName.Value = cardStatusName;

            var prmBalanceMin = new System.Data.SqlClient.SqlParameter("@p_BalanceMin", System.Data.SqlDbType.Int);
            prmBalanceMin.Value = Convert.ToInt32(balanceMin);

            var prmBalanceMax = new System.Data.SqlClient.SqlParameter("@p_BalanceMax", System.Data.SqlDbType.Int);
            prmBalanceMax.Value = Convert.ToInt32(balanceMax);

            var prmActivationDateBeg = new System.Data.SqlClient.SqlParameter("@p_ActivationDateBeg", System.Data.SqlDbType.DateTime);
            if (activationDateBeg == "")
            {
                prmActivationDateBeg.Value = new DateTime(2000, 1, 1);
            }
            else
            {
                prmActivationDateBeg.Value = activationDateBeg;
            }

            var prmActivationDateEnd = new System.Data.SqlClient.SqlParameter("@p_ActivationDateEnd", System.Data.SqlDbType.DateTime);
            if (activationDateEnd == "")
            {
                prmActivationDateEnd.Value = DateTime.Now;
            }
            else
            {
                prmActivationDateEnd.Value = activationDateEnd;
            }

            var prmActivationBy = new System.Data.SqlClient.SqlParameter("@p_ActivationBy", System.Data.SqlDbType.Int);
            prmActivationBy.Value = Convert.ToInt32(activationBy);

            var prmLastOperationDateBeg = new System.Data.SqlClient.SqlParameter("@p_LastOperationDateBeg", System.Data.SqlDbType.DateTime);
            if (lastOperationDateBeg == "")
            {
                prmLastOperationDateBeg.Value = new DateTime(2000, 1, 1);
            }
            else
            {
                prmLastOperationDateBeg.Value = lastOperationDateBeg;
            }
            
            var prmLastOperationDateEnd = new System.Data.SqlClient.SqlParameter("@p_LastOperationDateEnd", System.Data.SqlDbType.DateTime);
            if (lastOperationDateEnd == "")
            {
                prmLastOperationDateEnd.Value = DateTime.Now;
            }
            else
            {
                prmLastOperationDateEnd.Value = lastOperationDateEnd;
            }
           
            var prmLastOperationBy = new System.Data.SqlClient.SqlParameter("@p_LastOperationBy", System.Data.SqlDbType.Int);
            prmLastOperationBy.Value = Convert.ToInt32(lastOperationBy);

            var prmIncreaseSumMin = new System.Data.SqlClient.SqlParameter("@p_IncreaseSumMin", System.Data.SqlDbType.Int);
            prmIncreaseSumMin.Value = Convert.ToInt32(increaseSumMin);

            var prmIncreaseSumMax = new System.Data.SqlClient.SqlParameter("@p_IncreaseSumMax", System.Data.SqlDbType.Int);
            prmIncreaseSumMax.Value = Convert.ToInt32(increaseSumMax);

            var prmDecreaseSumMin = new System.Data.SqlClient.SqlParameter("@p_DecreaseSumMin", System.Data.SqlDbType.Int);
            prmDecreaseSumMin.Value = Convert.ToInt32(decreaseSumMin);

            var prmDecreaseSumMax = new System.Data.SqlClient.SqlParameter("@p_DecreaseSumMax", System.Data.SqlDbType.Int);
            prmDecreaseSumMax.Value = Convert.ToInt32(decreaseSumMax);

            var prmCountOperationMin = new System.Data.SqlClient.SqlParameter("@p_CountOperationMin", System.Data.SqlDbType.Int);
            prmCountOperationMin.Value = Convert.ToInt32(countOperationMin);

            var prmCountOperationMax = new System.Data.SqlClient.SqlParameter("@p_CountOperationMax", System.Data.SqlDbType.Int);
            prmCountOperationMax.Value = Convert.ToInt32(countOperationMax);

            var result = db.Database
                .SqlQuery<GetCardList_Result>(
                    "GetCardList @p_Phone, @p_CardNum, @p_CardTypeCode, @p_CardStatusName, @p_BalanceMin," +
                    "@p_BalanceMax, @p_ActivationDateBeg, @p_ActivationDateEnd, @p_ActivationBy, @p_LastOperationDateBeg, " +
                    "@p_LastOperationDateEnd, @p_LastOperationBy, @p_IncreaseSumMin, @p_IncreaseSumMax, @p_DecreaseSumMin, " +
                    "@p_DecreaseSumMax, @p_CountOperationMin, @p_CountOperationMax",
                    prmPhone, prmCardNum, prmCardTypeCode, prmCardStatusName, prmBalanceMin, prmBalanceMax, prmActivationDateBeg, 
                    prmActivationDateEnd, prmActivationBy, prmLastOperationDateBeg, prmLastOperationDateEnd, prmLastOperationBy, 
                    prmIncreaseSumMin, prmIncreaseSumMax, prmDecreaseSumMin, prmDecreaseSumMax, prmCountOperationMin, prmCountOperationMax)
                .ToList();

            resultset = result;

            return resultset;
        }

        private void Diapasons()
        {
            var prmPhone = new System.Data.SqlClient.SqlParameter("@p_Phone", System.Data.SqlDbType.NVarChar);
            prmPhone.Value = "";

            var prmCardNum = new System.Data.SqlClient.SqlParameter("@p_CardNum", System.Data.SqlDbType.NVarChar);
            prmCardNum.Value = "";

            var prmCardTypeCode = new System.Data.SqlClient.SqlParameter("@p_CardTypeCode", System.Data.SqlDbType.NVarChar);
            prmCardTypeCode.Value = "";

            var prmCardStatusName = new System.Data.SqlClient.SqlParameter("@p_CardStatusName", System.Data.SqlDbType.NVarChar);
            prmCardStatusName.Value = "";

            var prmBalanceMin = new System.Data.SqlClient.SqlParameter("@p_BalanceMin", System.Data.SqlDbType.Int);
            prmBalanceMin.Value = 0;

            var prmBalanceMax = new System.Data.SqlClient.SqlParameter("@p_BalanceMax", System.Data.SqlDbType.Int);
            prmBalanceMax.Value = 0;

            var prmActivationDateBeg = new System.Data.SqlClient.SqlParameter("@p_ActivationDateBeg", System.Data.SqlDbType.DateTime);
            prmActivationDateBeg.Value = new DateTime(2000, 1, 1);

            var prmActivationDateEnd = new System.Data.SqlClient.SqlParameter("@p_ActivationDateEnd", System.Data.SqlDbType.DateTime);
            prmActivationDateEnd.Value = DateTime.Now;

            var prmActivationBy = new System.Data.SqlClient.SqlParameter("@p_ActivationBy", System.Data.SqlDbType.Int);
            prmActivationBy.Value = 0;

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
                .SqlQuery<GetCardListMinMaxDiapasons_Result>(
                    "GetCardListMinMaxDiapasons @p_Phone, @p_CardNum, @p_CardTypeCode, @p_CardStatusName, @p_BalanceMin," +
                    "@p_BalanceMax, @p_ActivationDateBeg, @p_ActivationDateEnd, @p_ActivationBy, @p_LastOperationDateBeg, " +
                    "@p_LastOperationDateEnd, @p_LastOperationBy, @p_IncreaseSumMin, @p_IncreaseSumMax, @p_DecreaseSumMin, " +
                    "@p_DecreaseSumMax, @p_CountOperationMin, @p_CountOperationMax",
                    prmPhone, prmCardNum, prmCardTypeCode, prmCardStatusName, prmBalanceMin, prmBalanceMax, prmActivationDateBeg,
                    prmActivationDateEnd, prmActivationBy, prmLastOperationDateBeg, prmLastOperationDateEnd, prmLastOperationBy,
                    prmIncreaseSumMin, prmIncreaseSumMax, prmDecreaseSumMin, prmDecreaseSumMax, prmCountOperationMin, prmCountOperationMax)
                .ToList();

            ViewBag.DiapasonBalanceMin = result[0].BalanceMin;
            ViewBag.DiapasonBalanceMax = result[0].BalanceMax;
            ViewBag.DiapasonIncreaseSumMin = result[0].IncreaseSumMin;
            ViewBag.DiapasonIncreaseSumMax = result[0].IncreaseSumMax;
            ViewBag.DiapasonDecreaseSumMin = result[0].DecreaseSumMin;
            ViewBag.DiapasonDecreaseSumMax = result[0].DecreaseSumMax;
            ViewBag.DiapasonCountOperationMin = result[0].CountOperationMin;
            ViewBag.DiapasonCountOperationMax = result[0].CountOperationMax;
        }

        // GET: Cards
        public ActionResult Index()
        {
            return View();
        }
    }
}