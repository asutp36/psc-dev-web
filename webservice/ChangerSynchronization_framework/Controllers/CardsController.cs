using ChangerSynchronization_framework.Controllers.Supplies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChangerSynchronization_framework.Controllers
{
    [Route("api/cards")]
    public class CardsController : ApiController
    {
        /// <summary>
        /// Запись события о выпущенной карте
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Запись создана</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="400">Модель не прошла влидацию</response>
        [HttpPost]
        public HttpResponseMessage Post([FromBody]NewCard model)
        {
            Logger.InitLogger();

            if (!ModelState.IsValid)
            {
                Logger.Log.Error("CardsPost: модель не прошла валидацию\n" + JsonConvert.SerializeObject(model) + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try 
            {
                return Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                Logger.Log.Error("CardsPost: произошла внутренняя ошибка\n" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
