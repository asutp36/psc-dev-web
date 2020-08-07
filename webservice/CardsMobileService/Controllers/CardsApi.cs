﻿using CardsMobileService.Controllers.Supplies;
using CardsMobileService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers
{
    public class CardsApi
    {
        private ModelDbContext _model = new ModelDbContext();

        public void WriteIncrease(IncreaseFromChanger model) 
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
                        $"VALUES ((select IDDevice from Device where Code = '{model.changer}'), (select IDOperationType from OperationTypes where Code = 'increase'), " +
                        $"(select IDCard from Cards where CardNum = '{model.cardNum}'), '{model.dtime}', {model.amount}, " +
                        $"({commandBalance}) + {model.amount}, (select IDDevice from Device where Code = '{model.changer}'), {model.localizedID})";
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
                }

                int id = _model.Database.ExecuteSqlRaw($"SELECT MAX(IDOperation) from Operations o join Cards c on c.IDCard = o.IDCard where c.CardNum = '{model.cardNum}'");
                int i = _model.Operations.Where(o => o.IdcardNavigation.CardNum.Equals(model.cardNum)).Max(o => o.Idoperation);
            }
        }

        public void GetBalance(string cardNum) { }

        public void UpdatePhone() { }

        public void WriteNewCard() { }

        public void GetCardsByPhone(string phone) { }
    }
}
