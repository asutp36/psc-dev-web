using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        List<Device> _devices = null;
        List<OperationType> _operationTypes = null;

        List<Device> _requiredPosts = null;
        List<Device> _requiredChangers = null;

        public CardsController()
        {
            _cardTypes = db.CardTypes.ToList();
            _cardStatuses = db.CardStatuses.ToList();
            _devices = db.Devices.ToList();
            _operationTypes = db.OperationTypes.ToList();

            _requiredPosts = new List<Device>();
            _requiredChangers = new List<Device>();

            var washes = db.Washes.Where(w => w.Code == "М13" || w.Code == "М14").ToList();
            var changers = db.Changers.ToList();

            foreach (Wash w in washes)
            {
                foreach (var p in w.Posts)
                {
                    var dev = _devices.Find(d => d.IDDevice == p.IDDevice);
                    if (dev.IDDeviceType == 2)
                    {
                        _requiredPosts.Add(dev);
                    }
                }

                var chs = changers.FindAll(c => c.IDWash == w.IDWash);
                
                foreach(var c in chs)
                {
                    var device = _devices.Find(d => d.IDDevice == c.IDDevice);

                    if (device != null)
                    {
                        _requiredChangers.Add(device);
                    }
                }
            }

            if(_requiredChangers.Count < 1)
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

            ViewBag.CardTypes = _cardTypes;
            ViewBag.CardStatuses = _cardStatuses;
            ViewBag.Changers = _requiredChangers;
            ViewBag.Posts = _requiredPosts;
            ViewBag.OperationTypes = _operationTypes;
        }

        public ActionResult Cards(string begTimeLO, string endTimeLO, string begTimeActivation, string endTimeActivation)
        {
            DateTime startDTimeLO;
            if (!DateTime.TryParse(begTimeLO, out startDTimeLO))
                startDTimeLO = new DateTime(2019, 1, 1);
                //startDTimeLO = DateTime.Today.AddDays(-1);

            DateTime stopDTimeLO;
            if (!DateTime.TryParse(endTimeLO, out stopDTimeLO))
                stopDTimeLO = DateTime.Today.AddSeconds(-1);

            DateTime startDTimeActivation;
            if (!DateTime.TryParse(begTimeActivation, out startDTimeActivation))
                startDTimeActivation = new DateTime(2019, 1, 1);
                //startDTimeActivation = DateTime.Today.AddMonths(-3);

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
            ViewBag.activationBy = "";
            ViewBag.lastOperationDateBeg = startDTimeLO.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.lastOperationDateEnd = stopDTimeLO.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.lastOperationBy = "";
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
            List<GetCardList_Result> viewList = GetCardsFromDB(
                phone, 
                cardNum, 
                cardTypeCode, 
                cardStatusName, 
                balanceMin,
                balanceMax, 
                activationDateBeg, 
                activationDateEnd, 
                activationBy, 
                lastOperationDateBeg, 
                lastOperationDateEnd,
                lastOperationBy, 
                increaseSumMin, 
                increaseSumMax, 
                decreaseSumMin, 
                decreaseSumMax, 
                countOperationMin, 
                countOperationMax
                );

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
                prmActivationDateBeg.Value = new DateTime(2019, 1, 1);
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

            var prmCodeActivationBy = new System.Data.SqlClient.SqlParameter("@p_CodeActivationBy", System.Data.SqlDbType.NVarChar);
            prmCodeActivationBy.Value = activationBy;

            var prmLastOperationDateBeg = new System.Data.SqlClient.SqlParameter("@p_LastOperationDateBeg", System.Data.SqlDbType.DateTime);
            if (lastOperationDateBeg == "")
            {
                prmLastOperationDateBeg.Value = new DateTime(2019, 1, 1);
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
           
            var prmCodeLastOperationBy = new System.Data.SqlClient.SqlParameter("@p_CodeLastOperationBy", System.Data.SqlDbType.NVarChar);
            prmCodeLastOperationBy.Value = lastOperationBy;

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
                    "@p_BalanceMax, @p_ActivationDateBeg, @p_ActivationDateEnd, @p_CodeActivationBy, @p_LastOperationDateBeg, " +
                    "@p_LastOperationDateEnd, @p_CodeLastOperationBy, @p_IncreaseSumMin, @p_IncreaseSumMax, @p_DecreaseSumMin, " +
                    "@p_DecreaseSumMax, @p_CountOperationMin, @p_CountOperationMax",
                    prmPhone, prmCardNum, prmCardTypeCode, prmCardStatusName, prmBalanceMin, prmBalanceMax, prmActivationDateBeg, 
                    prmActivationDateEnd, prmCodeActivationBy, prmLastOperationDateBeg, prmLastOperationDateEnd, prmCodeLastOperationBy, 
                    prmIncreaseSumMin, prmIncreaseSumMax, prmDecreaseSumMin, prmDecreaseSumMax, prmCountOperationMin, prmCountOperationMax)
                .ToList();

            resultset = result;

            return resultset;
        }

        public ActionResult CurrentCardView(
            string phone, 
            string cardNum, 
            string cardTypeCode, 
            string cardStatusName
            )
        {
            ViewBag.Phone = phone;
            ViewBag.CardNum = cardNum;
            ViewBag.CardTypeCode = cardTypeCode;
            ViewBag.CardStatusName = cardStatusName;
            ViewBag.OperationDateBeg = new DateTime(2019, 1, 1).ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.OperationDateEnd = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.OperationTypeName = "";
            ViewBag.CodeOperationBy = "";
            ViewBag.LocalizedID = "";

            return View();
        }

        public ActionResult UpdateViewBagOperations(
            string phone,
            string cardNum,
            string cardTypeCode,
            string cardStatusname,
            string operationDateBeg,
            string operationDateEnd,
            string operationTypeName,
            string codeOperationBy
            )
        {
            List<GetCardsOperations_Result> viewList = GetOperationsFromDB(
                phone,
                cardNum, 
                cardTypeCode,
                cardStatusname,
                operationDateBeg,
                operationDateEnd,
                operationTypeName,
                codeOperationBy
                );

            return PartialView("_CurrentCardList", viewList);
        }

        private List<GetCardsOperations_Result> GetOperationsFromDB(
            string phone,
            string cardNum,
            string cardTypeCode,
            string cardStatusName,
            string operationDateBeg,
            string operationDateEnd,
            string operationTypeName,
            string codeOperationBy
            )
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
            if (operationDateBeg == "")
            {
                prmOperationDateBeg.Value = new DateTime(2019, 1, 1);
            }
            else
            {
                prmOperationDateBeg.Value = operationDateBeg;
            }

            var prmOperationDateEnd = new System.Data.SqlClient.SqlParameter("@p_OperationDateEnd", System.Data.SqlDbType.DateTime);
            if (operationDateEnd == "")
            {
                prmOperationDateEnd.Value = DateTime.Now;
            }
            else
            {
                prmOperationDateEnd.Value = operationDateEnd;
            }

            var prmCodeOperationBy = new System.Data.SqlClient.SqlParameter("@p_CodeOperationBy", System.Data.SqlDbType.NVarChar);
            prmCodeOperationBy.Value = codeOperationBy;

            var prmLocalizedID = new System.Data.SqlClient.SqlParameter("@p_LocalizedID", System.Data.SqlDbType.NVarChar);
            prmLocalizedID.Value = "";

            var result = db.Database
                .SqlQuery<GetCardsOperations_Result>(
                    "GetCardsOperations @p_Phone, @p_CardNum, @p_CardTypeCode, @p_CardStatusName, " +
                    "@p_OperationTypeName, @p_OperationDateBeg, @p_OperationDateEnd, @p_CodeOperationBy, @p_LocalizedID",
                    prmPhone, prmCardNum, prmCardTypeCode, prmCardStatusName, prmOperationTypeName,
                    prmOperationDateBeg, prmOperationDateEnd, prmCodeOperationBy, prmLocalizedID)
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
            prmActivationDateBeg.Value = new DateTime(2019, 1, 1);

            var prmActivationDateEnd = new System.Data.SqlClient.SqlParameter("@p_ActivationDateEnd", System.Data.SqlDbType.DateTime);
            prmActivationDateEnd.Value = DateTime.Now;

            var prmCodeActivationBy = new System.Data.SqlClient.SqlParameter("@p_CodeActivationBy", System.Data.SqlDbType.NVarChar);
            prmCodeActivationBy.Value = "";

            var prmLastOperationDateBeg = new System.Data.SqlClient.SqlParameter("@p_LastOperationDateBeg", System.Data.SqlDbType.DateTime);
            prmLastOperationDateBeg.Value = new DateTime(2019, 1, 1);

            var prmLastOperationDateEnd = new System.Data.SqlClient.SqlParameter("@p_LastOperationDateEnd", System.Data.SqlDbType.DateTime);
            prmLastOperationDateEnd.Value = DateTime.Now;

            var prmCodeLastOperationBy = new System.Data.SqlClient.SqlParameter("@p_CodeLastOperationBy", System.Data.SqlDbType.NVarChar);
            prmCodeLastOperationBy.Value = "";

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
                    "@p_BalanceMax, @p_ActivationDateBeg, @p_ActivationDateEnd, @p_CodeActivationBy, @p_LastOperationDateBeg, " +
                    "@p_LastOperationDateEnd, @p_CodeLastOperationBy, @p_IncreaseSumMin, @p_IncreaseSumMax, @p_DecreaseSumMin, " +
                    "@p_DecreaseSumMax, @p_CountOperationMin, @p_CountOperationMax",
                    prmPhone, prmCardNum, prmCardTypeCode, prmCardStatusName, prmBalanceMin, prmBalanceMax, prmActivationDateBeg,
                    prmActivationDateEnd, prmCodeActivationBy, prmLastOperationDateBeg, prmLastOperationDateEnd, prmCodeLastOperationBy,
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
    }
}