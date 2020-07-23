using ChangerSynchronization_framework.Controllers.Supplies;
using ChangerSynchronization_framework.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChangerSynchronization_framework.Controllers
{
    public class SyncController : ApiController
    {
        private ModelDb _model = new ModelDb();
        public HttpResponseMessage PostEventChanger([FromBody]EventChangerFull model)
        {
            Logger.InitLogger();

            try
            {
                Logger.Log.Debug("PostEventChanger: запуск с параметрами" + JsonConvert.SerializeObject(model));

                if (!ModelState.IsValid)
                {
                    Logger.Log.Error("PostEventChanger: модель не прошла валидацию." + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }

                if (!_model.Database.Exists())
                {
                    Logger.Log.Error("PostEventChanger: база данных не найдена" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }



                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Logger.Log.Error("PostEventChanger: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
