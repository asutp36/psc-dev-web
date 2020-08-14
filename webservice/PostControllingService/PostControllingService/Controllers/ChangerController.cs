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
        [HttpGet]
        [ActionName("counters")]
        public HttpResponseMessage GetCounters(string changer)
        {
            Logger.InitLogger();

            Logger.Log.Debug("GetCounters: запуск с параметром\n" + changer);

            if (changer.Length == 0)
            {
                Logger.Log.Error("GetCounters: входное значение некорректное" + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            string ip = _model.Device.Where(d => d.Code.Equals(changer)).FirstOrDefault().IpAddress;

            if (ip == null || ip.Length == 0)
            {
                Logger.Log.Error("GetCounters: разменник не найден" + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            HttpSenderResponse response = HttpSender.SendGet("http://" + ip + "/counters");

            Logger.Log.Debug("GetCounters: ответ от разменника:\n" + JsonConvert.SerializeObject(response));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ChangerCounters counters = JsonConvert.DeserializeObject<ChangerCounters>(response.Message);
                return Request.CreateResponse(HttpStatusCode.OK, counters);
            }

            return Request.CreateResponse((HttpStatusCode)424);
        }
    }
}
