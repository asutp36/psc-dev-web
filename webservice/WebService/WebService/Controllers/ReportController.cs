using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService.Controllers.Supplies;
using System.Web;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using WebService.Models;
using System.Data.Common;

namespace WebService.Controllers
{
    public class ReportController : ApiController
    {
        private ModelDb _model = new ModelDb();

        [HttpPost]
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

                foreach (string file in request.Files)
                {
                    var postedFile = request.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/" + "\\Uploads\\" + postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    Logger.Log.Debug("File saved to: " + filePath.ToString());
                }

                var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                //responseGood.Headers.Add("ServerID", serverID.ToString());
                return responseGood;
            }

            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message.ToString());
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        private void ParseXls(string path)
        {
            Logger.InitLogger();
            try
            {
                Excel.Application excel = new Excel.Application();
                Excel.Workbook workbook = excel.Workbooks.Open(@"D:\Programs\Работа\psc-dev-web\TryParseReports\m8-08.10.2019.xls");
                Excel.Worksheet worksheet = workbook.Worksheets["Данные"] as Excel.Worksheet;

                object[,] data = worksheet.UsedRange.Value;

                for (int r = 3; r < 10; r++)
                {
                    DateTime startDate = DateTime.Parse(data[r, 1].ToString());
                    DateTime stopDate = DateTime.Parse(data[r, 2].ToString());
                    DateTime vupdatetime2 = DateTime.Parse(data[r, 9].ToString());

                    DbCommand command = _model.Database.Connection.CreateCommand();

                    command.CommandText = "INSERT INTO %table% (startDate, stopDate, regionid, regionname, washname, wshaddress, washnum, postname, vupdatetime2, " +
                        "msgtotal, b10, b50, b100, b500, b1k, m10, s, linkcontext)" +
                        $" VALUES({startDate.ToString("yyyyMMdd")}, " +
                        $"{stopDate.ToString("yyyyMMdd")}, {data[r, 3]}, \'{data[r, 4]}\', {data[r, 5]}, {data[r, 6]}, {data[r, 7]}, {data[r, 8]}, " +
                        $"{vupdatetime2.ToString("yyyyMMdd HH:mm:ss.fff")}, {data[r, 10]}, {data[r, 11]}, {data[r, 12]}, {data[r, 13]}, {data[r, 14]}, {data[r, 15]}," +
                        $" {data[r, 16]}, {data[r, 17]}, {data[r, 18]});" + " SELECT SCOPE_IDENTITY()";
                    Logger.Log.Debug("Command is: " + command.CommandText);

                    var id = command.ExecuteScalar();
                }

                workbook.Close();
                excel.Quit();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message.ToString());
            }
        }
    }
}