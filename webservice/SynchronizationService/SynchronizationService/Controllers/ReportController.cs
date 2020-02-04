using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SynchronizationService.Controllers.Supplies;
using SynchronizationService.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace SynchronizationService.Controllers
{
    public class ReportController : ApiController
    {
        ModelDb _model = new ModelDb();

        [HttpPost]
        [ActionName("xls")]
        public HttpResponseMessage PostReport()
        {
            Logger.InitLogger();

            try
            {
                var request = HttpContext.Current.Request;

                if (request.Files.Count < 1)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }

                var filePath = "";
                foreach (string file in request.Files)
                {
                    var postedFile = request.Files[file];
                    filePath = HttpContext.Current.Server.MapPath("~/" + "\\Uploads\\" + postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    Logger.Log.Debug("File saved to: " + filePath.ToString());

                }
                ParseXls(filePath.ToString());

                var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                return responseGood;
            }

            catch (Exception ex)
            {
                Logger.Log.Error("Reciever: " + ex.Message.ToString());
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        private void ParseXls(string path)
        {
            Logger.InitLogger();
            Excel.Application excel = new Excel.Application();
            Excel.Workbook workbook = excel.Workbooks.Open(path);
            try
            {
                Excel.Worksheet worksheet = workbook.Worksheets["Данные"] as Excel.Worksheet;

                object[,] data = worksheet.UsedRange.Value;

                _model.Database.Connection.Open();
                Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                for (int r = 3; r < 10; r++)
                {
                    DateTime startDate = DateTime.Parse(data[r, 1].ToString());
                    DateTime stopDate = DateTime.Parse(data[r, 2].ToString());
                    DateTime vupdatetime2 = DateTime.Parse(data[r, 9].ToString());

                    DbCommand command = _model.Database.Connection.CreateCommand();

                    //знать какое из значений даты вставлять

                    command.CommandText = "INSERT INTO CountersTotalPre (IDPost, DTime, " +
                        "b10, b50, b100, b500, b1k, m10)" +
                        $" VALUES((SELECT IDPost FROM Posts WHERE Code = 'М{data[r, 7].ToString().Trim()}-{data[r, 8].ToString().Trim()}'), " +
                        $"\'{vupdatetime2.ToString("yyyyMMdd HH:mm:ss.fff")}\', {data[r, 11]}, {data[r, 12]}, {data[r, 13]}, {data[r, 14]}, {data[r, 15]}," +
                        $" {data[r, 16]});" + " SELECT SCOPE_IDENTITY()";
                    Logger.Log.Debug("Command is: " + command.CommandText);
                    //Logger.Log.Debug("Timecode is: " + vupdatetime2.ToString("yyyyMMdd HH:mm:ss.fff"));

                    var id = command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Parser: " + ex.Message.ToString());
            }
            finally
            {
                _model.Database.Connection.Close();

                workbook.Close();
                excel.Quit();
            }
        }
    }
}
