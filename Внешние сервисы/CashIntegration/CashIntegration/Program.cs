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
            Logger.InitLogger();

            using (ModelDb model = new ModelDb())
            {
                

                while (true)
                {
                    Console.Clear();
                    try
                    {
                        List<CashWId> cashes = new List<CashWId>();

                        cashes = GetDataToSend();
                        if (cashes != null)
                        {
                            using (DbConnection con = model.Database.Connection)
                            {
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
                                    Console.WriteLine(response.StatusCode);
                                    //IntegrationResponse response = new IntegrationResponse();
                                    //response.StatusCode = 200;
                                    //response.TransactionId = 88;

                                    comm.CommandText = $"INSERT INTO CashIntegration (IDEvent, ServerID, ServerMessage, ServerStatus) " +
                                    $"VALUES ({c.ID}, {response.TransactionId}, '{response.Message}', {response.StatusCode})";
                                    comm.ExecuteScalar();
                                }
                                con.Close();
                                cashes.Clear();
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Logger.Log.Error(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    }
                    Console.WriteLine(GetLastTime());
                    Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    Console.WriteLine("sleeping...z.z.z.");
                    System.Threading.Thread.Sleep(60000);
                }
            }

        }

        public static List<CashWId> GetDataToSend()
        {
            Logger.InitLogger();

            using (ModelDb model = new ModelDb())
            {

                try
                {
                    List<CashWId> cash = new List<CashWId>();

                    DbConnection con1 = model.Database.Connection;
                    con1.Open();
                    Logger.Log.Debug("Подключение к БД: " + con1.State);

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
                        " and ci.IDEvent is NULL";
                    DbDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                        while (reader.Read())
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

                    Logger.Log.Debug("Считано строк: " + cash.Count + Environment.NewLine);
                    reader.Close();
                    con1.Close();
                    return cash;
                }
                catch (Exception e)
                {
                    Logger.Log.Error("Ошибка: " + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    model.Database.Connection.Close();
                    return null;
                }
            }
        }

        public static string GetLastTime()
        {
            string result = "";

            Logger.InitLogger();

            try
            {
                ModelDb model = new ModelDb();

                DbConnection con = model.Database.Connection;
                con.Open();

                DbCommand command = con.CreateCommand();

                command.CommandText = "select " +
                    "p.Code" +
                    ", max(e.DTime) time " +
                    "from CashIntegration ci " +
                    "join Event e on e.IDEvent = ci.IDEvent " +
                    "join Posts p on p.IDPost = e.IDPost " +
                    "group by p.Code " +
                    "order by p.Code";

                DbDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result += reader.GetString(reader.GetOrdinal("Code")) + ": " + reader.GetDateTime(reader.GetOrdinal("time")) + Environment.NewLine;
                    }
                }

            }
            catch(Exception e)
            {
                Logger.Log.Error(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
            }

            return result;
        }
    }
}
