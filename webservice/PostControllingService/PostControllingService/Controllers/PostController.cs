using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using PostControllingService.Controllers.Supplies;

namespace PostControllingService.Controllers
{
    public class PostController : ApiController
    {
        [HttpPost]
        [ActionName("price")]
        public HttpResponseMessage SendPrice([FromBody]ChangePricesData change)
        {
            Logger.InitLogger();

            try
            {
                if (change != null)
                {

                    foreach (Price p in change.Prices)
                    {
                        Logger.Log.Debug("Изменение тарифа. Отправка на пост: " + p);
                        SendPostResponse response = HttpSender.SendPost("http://109.196.164.28:5000/api/post/rate" , JsonConvert.SerializeObject(p));

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            Logger.Log.Error(String.Format("Ответ сервера: {0}\n{1}", response.StatusCode, response.Message));

                            return Request.CreateResponse(HttpStatusCode.Conflict);
                        }
                    }

                    Logger.Log.Debug(String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), "Отправлено успешно") + Environment.NewLine);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);
                return responseBad;
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ActionName("incrbalance")]
        public HttpResponseMessage IncreaseBalance([FromBody]IncreaseBalance balance)
        {
            Logger.InitLogger();

            try
            {
                if (balance != null)
                {
                    Logger.Log.Debug("Пополнение баланса. Отправка на пост: " + balance.ToString());

                    SendPostResponse response = HttpSender.SendPost("http://109.196.164.28:5000/api/post/balance/increase", JsonConvert.SerializeObject(balance));

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Log.Error(String.Format("Ответ сервера: {0}\n{1}", response.StatusCode, response.Message));

                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    }

                    Logger.Log.Debug(String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), "Отправлено успешно") + Environment.NewLine);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);
                return responseBad;
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ActionName("getbalance")]
        public HttpResponseMessage GetCurrentBalance([FromBody]RequestWPostCode post)
        {
            Logger.InitLogger();

            try
            {
                if(post != null)
                {
                    Logger.Log.Debug(String.Format("GetBalace: Запуск с параметрами:\nPost: {0}", post.Post));

                    int balance = HttpSender.GetInt("http://109.196.164.28:5000/api/post/balance/get");
                    if(balance == -1)
                    {
                        Logger.Log.Error("Произошла ошибка при отправке запроса. Ответ -1");
                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    }
                    else
                    {
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Balance", balance.ToString());
                        return response;
                    }
                }
                else
                {
                    Logger.Log.Error("Post == null. Ошибка в данных запроса");
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ActionName("getfunc")]
        public HttpResponseMessage GetCurrentFunction([FromBody]RequestWPostCode post)
        {
            Logger.InitLogger();

            try
            {
                if (post != null)
                {
                    Logger.Log.Debug(String.Format("GetCurrentFunction: Запуск с параметрами:\nPost: {0}", post.Post));

                    string result = HttpSender.GetString("http://109.196.164.28:5000/api/post/func/get");
                    PostFunction func = JsonConvert.DeserializeObject<PostFunction>(result);
                    if (func == null)
                    {
                        Logger.Log.Error("Произошла ошибка при отправке запроса. Ответ null");
                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    }
                    else
                    {
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Function", func.Name);
                        return response;
                    }
                }
                else
                {
                    Logger.Log.Error("Post == null. Ошибка в данных запроса");
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }


        [HttpPost]
        [ActionName("setfunc")]
        public HttpResponseMessage SetFunction([FromBody]SetFunction func)
        {
            Logger.InitLogger();

            try
            {
                if(func != null)
                {
                    Logger.Log.Debug(String.Format("SetFunction: Запуск с параметрами:\nPost: {0}, Function: {1}, Login: {2}", func.Post, func.Function, func.Login));

                    SendPostResponse response = HttpSender.SendPost("http://109.196.164.28:5000/api/post/func/set", JsonConvert.SerializeObject(func));

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Log.Error(String.Format("Ответ сервера: {0}\n{1}", response.StatusCode, response.Message));

                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    }

                    Logger.Log.Debug(String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), "Отправлено успешно") + Environment.NewLine);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    Logger.Log.Error("Function == null. Ошибка в данных запроса");
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ActionName("heartbeat")]
        public HttpResponseMessage HeartBeat([FromBody]RequestWPostCode post)
        {
            Logger.InitLogger();

            try
            {
                if (post != null)
                {
                    Logger.Log.Debug("HeartBeat");

                    int heartbeat = HttpSender.GetInt("http://109.196.164.28:5000/api/post/heartbeat");

                    if (heartbeat == -1)
                    {
                        Logger.Log.Error("HeartBeat: Произошла ошибка при отправке запроса. Ответ -1");
                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    }
                    else
                    {
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("HeartBeat", heartbeat.ToString());
                        return response;
                    }
                }
                else
                {
                    Logger.Log.Error("Post == null. Ошибка в данных запроса");
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
