using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashIntegration.Models;
using CashIntegration.Supplies;
using Newtonsoft.Json;

namespace CashIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            ModelDb model = new ModelDb();

            #region предыдущая версия
            //while (true)
            //{
            //    DbConnection con1 = model.Database.Connection;
            //    con1.Open();
            //    DbCommand command = con1.CreateCommand();

            //    command.CommandText = "select" +
            //        " * " +
            //        "from " +
            //        "(" +
            //        "select" +
            //        " e.IDEvent, p.Code, e.DTime, ei.amount " +
            //        "from EventIncrease ei " +
            //        "join Event e on e.IDEvent = ei.IDEvent " +
            //        "join Posts p on e.IDPost = p.IDPost " +
            //        ") t" +
            //        " left join CashIntegration ci on ci.IDEvent = t.IDEvent " +
            //        "where 1=1" +
            //        " and ci.IDEvent is NULL " +
            //        "and t.DTime < GETDATE() - 1";

            //    DbDataReader reader = command.ExecuteReader();

            //    if (!reader.HasRows)
            //    {
            //        reader.Close();
            //        con1.Close();
            //        break;
            //    }

            //    reader.Read();

            //    Cash c = new Cash();
            //    c.Code = reader.GetString(reader.GetOrdinal("Code"));
            //    c.Date = reader.GetDateTime(reader.GetOrdinal("DTime"));
            //    int amount = reader.GetInt32(reader.GetOrdinal("Amount"));
            //    int id = reader.GetInt32(reader.GetOrdinal("IDEvent"));

            //    switch (amount)
            //    {
            //        case 10:
            //            c.CashIncome.Add(new CashIncome("m10", 1));
            //            break;
            //        case 50:
            //            c.CashIncome.Add(new CashIncome("b50", 1));
            //            break;
            //        case 100:
            //            c.CashIncome.Add(new CashIncome("b100", 1));
            //            break;
            //        case 200:
            //            c.CashIncome.Add(new CashIncome("b200", 1));
            //            break;
            //    }
            //    reader.Close();
            //    con1.Close();
            //    string data = JsonConvert.SerializeObject(c);
            //    IntegrationResponse response = HttpSender.SendCash("https://api.myeco24.ru/transactions/post/cash", data, true);
            //    //IntegrationResponse response = new IntegrationResponse();
            //    //response.StatusCode = 200;
            //    //response.TransactionId = 88;

            //    DbConnection con2 = model.Database.Connection;
            //    con2.Open();

            //    DbCommand comm = con2.CreateCommand();

            //    comm.CommandText = $"INSERT INTO CashIntegration (IDEvent, ServerID, ServerMessage, ServerStatus) " +
            //    $"VALUES ({id}, {response.TransactionId}, '{response.Message}', {response.StatusCode})";
            //    comm.ExecuteScalar();
            //    con2.Close();
            //}
            //model.Database.Connection.Close();
            #endregion


            List<CashWId> cashes = GetDataToSend();
            DbConnection con = model.Database.Connection;
            con.Open();

            DbCommand comm = con.CreateCommand();


            foreach (CashWId c in cashes)
            {
                Cash cash = new Cash();
                cash.Date = c.Date;
                cash.CashIncome = c.CashIncome;
                cash.Code = c.Code;
                
                string data = JsonConvert.SerializeObject(c);
                IntegrationResponse response = HttpSender.SendCash("https://api.myeco24.ru/transactions/post/cash", data, true);
                //IntegrationResponse response = new IntegrationResponse();
                //response.StatusCode = 200;
                //response.TransactionId = 88;

                comm.CommandText = $"INSERT INTO CashIntegration (IDEvent, ServerID, ServerMessage, ServerStatus) " +
                $"VALUES ({c.ID}, {response.TransactionId}, '{response.Message}', {response.StatusCode})";
                comm.ExecuteScalar();
            }
            con.Close();

        }

        public static List<CashWId> GetDataToSend()
        {
            List<CashWId> cash = new List<CashWId>();
            
            ModelDb model = new ModelDb();

            DbConnection con1 = model.Database.Connection;
            con1.Open();
            DbCommand command = con1.CreateCommand();

            command.CommandText = "select" +
                " * " +
                "from " +
                "(" +
                "select" +
                " e.IDEvent, p.Code, e.DTime, ei.amount " +
                "from EventIncrease ei " +
                "join Event e on e.IDEvent = ei.IDEvent " +
                "join Posts p on e.IDPost = p.IDPost " +
                ") t" +
                " left join CashIntegration ci on ci.IDEvent = t.IDEvent " +
                "where 1=1" +
                " and ci.IDEvent is NULL " +
                "and t.DTime < GETDATE() - 1";
            DbDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                
                CashWId c = new CashWId();
                c.Code = reader.GetString(reader.GetOrdinal("Code"));
                c.Date = reader.GetDateTime(reader.GetOrdinal("DTime"));
                int amount = reader.GetInt32(reader.GetOrdinal("Amount"));
                c.ID = reader.GetInt32(reader.GetOrdinal("IDEvent"));

                switch (amount)
                {
                    case 10:
                        c.CashIncome.Add(new CashIncome("m10", 1));
                        break;
                    case 50:
                        c.CashIncome.Add(new CashIncome("b50", 1));
                        break;
                    case 100:
                        c.CashIncome.Add(new CashIncome("b100", 1));
                        break;
                    case 200:
                        c.CashIncome.Add(new CashIncome("b200", 1));
                        break;
                }

                cash.Add(c);
            }

           
            reader.Close();
            con1.Close();
            return cash;
        }
    }
}
