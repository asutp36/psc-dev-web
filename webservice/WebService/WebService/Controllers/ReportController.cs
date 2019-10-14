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

namespace WebService.Controllers
{
    public class ReportController : ApiController
    {
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
            Excel.Application excel = new Excel.Application();

        }
    }
}