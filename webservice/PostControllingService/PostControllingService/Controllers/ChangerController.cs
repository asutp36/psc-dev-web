using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using PostControllingService.Controllers.Supplies;
using PostControllingService.Models;

namespace PostControllingService.Controllers
{
    public class ChangerController : ApiController
    {
        ModelDb _model = new ModelDb();

        /// <summary>
        /// Получить счётчики на разменнике
        /// </summary>
        /// <param name="changer">Код разменника</param>
        /// <returns></returns>
        /// <response code="200">Ок</response>
        /// <response code="400">Некорректное значение</response>
        /// <response code="424">Ответ разменника неудовлетворительный или нет связи</response>
        /// <response code="404">Разменник не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("api/changer/state/{changer}")]
        [ActionName("state")]
        public HttpResponseMessage GetState(string changer)
        {
            Logger.InitLogger();

            try
            {
                Logger.Log.Debug("GetState: запуск с параметром\n" + changer);

                if (changer.Length == 0)
                {
                    Logger.Log.Error("GetState: входное значение некорректное" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Некорректное значение");
                }

                Device devive = _model.Device.Where(d => d.Code.Equals(changer)).FirstOrDefault();

                if (devive == null)
                {
                    Logger.Log.Error("GetState: разменник не найден" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Разменник не найден");
                }

                HttpSenderResponse response = HttpSender.SendGet("http://" + devive.IpAddress + "/api/exch/get/counter");

                Logger.Log.Debug("GetState: ответ от разменника:\n" + JsonConvert.SerializeObject(response) + Environment.NewLine);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ChangerState state = JsonConvert.DeserializeObject<ChangerState>(response.ResultMessage);
                    return Request.CreateResponse(HttpStatusCode.OK, state);
                }

                return Request.CreateResponse((HttpStatusCode)424, "Нет связи с разменником");
            }
            catch(Exception e)
            {
                Logger.Log.Error("GetState: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
