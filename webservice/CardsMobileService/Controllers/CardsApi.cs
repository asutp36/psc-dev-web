using CardsMobileService.Controllers.Supplies;
using CardsMobileService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers
{
    public class CardsApi
    {
        private ModelDbContext _model = new ModelDbContext();
        private ILogger<CardsApi> _logger;

        public CardsApi(ILogger<CardsApi> logger)
        {
            _logger = logger;
        }

        public int WriteIncrease(IncreaseFromChanger model) 
        {
            if (_model.Database.CanConnect())
            {
                _model.Database.OpenConnection();

                try
                {
                    string commandBalance = $"select top 1 " +
                            $"isnull(o.Balance, 0) " +
                            $"from Cards c " +
                            $"left join Operations o on o.IDCard = c.IDCard " +
                            $"and o.DTime = (select MAX(DTime) from Operations where IDCard = c.IDCard) " +
                            $"where c.CardNum = '{model.cardNum}' " +
                            $"order by o.IDOperation desc";

                    _model.Database.BeginTransaction();

                    string command = $"UPDATE Cards SET Balance = ({commandBalance}) + {model.amount} WHERE CardNum = '{model.cardNum}'";
                    _model.Database.ExecuteSqlRaw(command);

                    command = "INSERT INTO Operations (IDDevice, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID) " +
                        $"VALUES ((select IDDevice from Device where Code = '{model.changer}'), (select IDOperationType from OperationTypes where Code = '{model.operationType}'), " +
                        $"(select IDCard from Cards where CardNum = '{model.cardNum}'), '{model.dtime}', {model.amount}, " +
                        $"({commandBalance}) + {model.amount}, (select IDDevice from Device where Code = '{model.changer}'), {model.localizedID})";
                    
                    _logger.LogDebug("WriteIncrease: command is: " + command);

                    _model.Database.ExecuteSqlRaw(command);

                    _model.Database.CommitTransaction();

                }
                catch(Exception e)
                {
                    if (_model.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                    {
                        _model.Database.CloseConnection();
                    }

                    _model.Database.RollbackTransaction();

                    _logger.LogError("WriteIncrease: ошибка записи. " + e.Message + Environment.NewLine + e.StackTrace);

                    return -1;
                }

                int id = _model.Operations.Where(o => o.IdcardNavigation.CardNum.Equals(model.cardNum)).Max(o => o.Idoperation);

                return id;
            }

            throw new Exception("База данных не найдена");
        }

        public int WriteDecrease(IncreaseFromChanger model)
        {
            if (_model.Database.CanConnect())
            {
                _model.Database.OpenConnection();

                try
                {
                    string commandBalance = $"select top 1 " +
                            $"isnull(o.Balance, 0) " +
                            $"from Cards c " +
                            $"left join Operations o on o.IDCard = c.IDCard " +
                            $"and o.DTime = (select MAX(DTime) from Operations where IDCard = c.IDCard) " +
                            $"where c.CardNum = '{model.cardNum}' " +
                            $"order by o.IDOperation desc";

                    _model.Database.BeginTransaction();

                    string command = $"UPDATE Cards SET Balance = ({commandBalance}) + {model.amount} WHERE CardNum = '{model.cardNum}'";
                    _model.Database.ExecuteSqlRaw(command);

                    command = "INSERT INTO Operations (IDDevice, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID) " +
                        $"VALUES ((select IDDevice from Device where Code = '{model.changer}'), (select IDOperationType from OperationTypes where Code = '{model.operationType}'), " +
                        $"(select IDCard from Cards where CardNum = '{model.cardNum}'), '{model.dtime}', {model.amount}, " +
                        $"({commandBalance}) - {model.amount}, (select IDDevice from Device where Code = '{model.changer}'), {model.localizedID})";

                    _logger.LogDebug("WriteIncrease: command is: " + command);

                    _model.Database.ExecuteSqlRaw(command);

                    _model.Database.CommitTransaction();

                }
                catch (Exception e)
                {
                    if (_model.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                    {
                        _model.Database.CloseConnection();
                    }

                    _model.Database.RollbackTransaction();

                    _logger.LogError("WriteIncrease: ошибка записи. " + e.Message + Environment.NewLine + e.StackTrace);

                    return -1;
                }

                int id = _model.Operations.Where(o => o.IdcardNavigation.CardNum.Equals(model.cardNum)).Max(o => o.Idoperation);

                return id;
            }

            throw new Exception("База данных не найдена");
        }

        public int? GetBalance(string cardNum) 
        {
            return _model.Cards.Where(c => c.CardNum.Equals(cardNum)).FirstOrDefault().Balance;
        }

        public void UpdatePhone() { }

        public int WriteNewCard(NewCardFromChanger model) 
        {
            if (_model.Database.CanConnect())
            {
                PhoneFormatter formattedPhone = new PhoneFormatter(model.phone);

                _model.Database.OpenConnection();

                try
                {
                    _model.Database.BeginTransaction();

                    string command = $"insert into Owners (Phone, PhoneInt, LocalizedBy, LocalizedID) values ('{formattedPhone.phone}', " +
                        $"{formattedPhone.phoneInt}, (select IDDevice from Device where Code = '{model.changer}'), {model.localizedID})";
                    _logger.LogInformation("WriteNewCard: command is: " + command);
                    _model.Database.ExecuteSqlRaw(command);

                    command = $"insert into Cards (IDOwner, CardNum,  IDCardStatus, IDCardType, LocalizedBy, LocalizedID, Balance) " +
                        $"values (scope_identity(), '{model.cardNum}', (select IDCardStatus from CardStatuses cs where cs.Code = 'norm'), " +
                        $"(select IDCardType from CardTypes ct where ct.Code = 'client'), " +
                        $"(select IDDevice from Device where Code = '{model.changer}'), {model.localizedID}, {model.amount})";
                    _logger.LogInformation("WriteNewCard: command is: " + command);
                    _model.Database.ExecuteSqlRaw(command);

                    _logger.LogInformation("WriteNewCard: Добавлены Owner и Card. CardNum = " + model.cardNum);

                    command = $"insert into Operations (IDDevice, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID) " +
                                $"values ((select IDDevice from Device where Code = '{model.changer}'), " +
                                $"(select IDOperationType from OperationTypes ot where ot.Code = 'activation'), " +
                                $"scope_identity(), '{model.dtime}', " +
                                $"0, 0, (select IDDevice from Device where Code = '{model.changer}'), {model.localizedID});";
                    _logger.LogInformation("WriteNewCard: command is: " + command);
                    _model.Database.ExecuteSqlRaw(command);

                    _model.Database.CommitTransaction();
                }
                catch(Exception e)
                {
                    if (_model.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                    {
                        _model.Database.CloseConnection();
                    }

                    _model.Database.RollbackTransaction();

                    _logger.LogError("WriteNewCard: ошибка записи. " + e.Message + Environment.NewLine + e.StackTrace);

                    return -1;
                }

                return _model.Operations.Where(o => o.Idcard == _model.Cards.Where(c => c.CardNum.Equals(model.cardNum)).FirstOrDefault().Idcard).Max(o => o.Idoperation);
            }

            throw new Exception("База данных не найдена");
        }

        public List<string> GetCardsByPhone(string phone) 
        {
            PhoneFormatter formattedPhone = new PhoneFormatter(phone);

            List<Cards> cards = _model.Cards.Where(c => c.Idowner == _model.Owners.Where(o => o.PhoneInt.Equals(formattedPhone.phoneInt)).First().Idowner).ToList();
            List<string> nums = new List<string>();

            foreach (Cards c in cards)
                nums.Add(c.CardNum);

            return nums;
        }

        public bool IsExist(string cardNum)
        {
            return _model.Cards.Where(c => c.CardNum.Equals(cardNum)).ToList().Count > 0;
        }

        public string GetNewCardNum()
        {
            string commandCardNum = "select " +
                                "min(v.Num) as Num " +
                                "from NumsMobileCards v " +
                                "left join Cards c on c.CardNum = v.Num " +
                                "where c.CardNum is null";

            return _model.NumsMobileCards.FromSqlRaw(commandCardNum).First().Num;
        }

        public string SendCardToApp(NewCardFromChanger card)
        {
            NewCardToApp cardToApp = new NewCardToApp
            {
                card = card.cardNum,
                phone = card.phone,
                dtime = card.dtime,
                hash = CryptHash.GetHashCode(card.dtime)
            };

            HttpResponse response = HttpSender.SendPost("http://loyalty.myeco24.ru/api/externaldb/user-create", JsonConvert.SerializeObject(cardToApp));

            if (response.StatusCode == 0)
            {
                return "unavailible";
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return "ok";
            }

            return response.ResultMessage;
        }

        public string SendIncreaseToApp(IncreaseFromChanger model)
        {
            IncreaseToApp increaseToApp = new IncreaseToApp
            {
                card = model.cardNum,
                value = model.amount,
                wash_id = model.changer,
                operation_time = model.dtime,
                time_send = model.dtime,
                hash = CryptHash.GetHashCode(model.dtime)
            };

            HttpResponse response = HttpSender.SendPost("http://loyalty.myeco24.ru/api/externaldb/set-replenish", JsonConvert.SerializeObject(increaseToApp));

            if (response.StatusCode == 0)
            {
                return "unavailible";
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return "ok";
            }

            return response.ResultMessage;
        }

        public string GetPhone(string cardNum)
        {
            return _model.Owners.Find(_model.Cards.Where(c => c.CardNum.Equals(cardNum)).FirstOrDefault().Idowner).Phone;
        }

        public TechCards GetTechCards()
        {
            TechCards result = new TechCards();

            int idService = GetIDCardType("service");
            int idClean = GetIDCardType("clean");
            int idDoors = GetIDCardType("doors");
            int idCollect = GetIDCardType("collect");

            List<Cards> cards = _model.Cards.Where(c => !c.IdcardType.Equals(_model.CardTypes.Where(ct => ct.Code.Equals("client")).FirstOrDefault().IdcardType)).ToList();

            foreach(Cards c in cards)
            {
                if (c.IdcardType == idClean)
                    result.cleanUp.Add(c.CardNum);
                if (c.IdcardType == idCollect)
                    result.collect.Add(c.CardNum);
                if (c.IdcardType == idDoors)
                    result.doors.Add(c.CardNum);
                if (c.IdcardType == idService)
                    result.service.Add(c.CardNum);
            }

            return result;
        }

        private int GetIDCardType(string typeCode)
        {
            return _model.CardTypes.Where(ct => ct.Code.Equals(typeCode)).FirstOrDefault().IdcardType;
        }

        public string UpdateCardNum(ChangeCardNumModel change)
        {
            if (_model.Database.CanConnect())
            {
                _model.Database.OpenConnection();

                try
                {
                    string command = $"UPDATE Cards SET CardNum = '{change.newNum}', LocalizedBy = (select IDDevice from Device where Code = '{change.changer}') " +
                    $"WHERE CardNum = '{change.oldNum}'";
                    int rowAffected = _model.Database.ExecuteSqlRaw(command);
                    
                    if (rowAffected > 0)
                    {
                        return "ok";
                    }

                    return "not ok";
                }
                catch(Exception e)
                {
                    if (_model.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                    {
                        _model.Database.CloseConnection();
                    }

                    _model.Database.RollbackTransaction();

                    _logger.LogError("WriteIncrease: ошибка записи. " + e.Message + Environment.NewLine + e.StackTrace);

                    return e.Message;
                }
            }

            return "База данных не найдена";
        }
    }
}
