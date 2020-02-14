using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using PostControllingService.Controllers.Supplies;
using PostControllingService.Models;

namespace PostControllingService.Controllers
{
    public class PostController : ApiController
    {
        ModelDb _model = new ModelDb();

        //[HttpPost]
        //[ActionName("getrate")]
        //public HttpResponseMessage GetCurrentRate([FromBody]RequestWPostsCodes posts)
        //{
        //    Logger.InitLogger();
        //    try
        //    {
        //        if(posts != null)
        //        {
        //            Logger.Log.Debug("GetCurrentRate: запуск с параметрами:\n" + JsonConvert.SerializeObject(posts));

        //            foreach(string post in posts.Posts)
        //            {

        //            }
        //        }
        //        else
        //        {
        //            Logger.Log.Error("GetCurrentRate: posts == null. Ошибка в данных запроса" + Environment.NewLine);
        //            return Request.CreateResponse(HttpStatusCode.NoContent);
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        Logger.Log.Error("GetCurrentRate: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }
        //}

        [HttpPost]
        [ActionName("price")]
        public HttpResponseMessage SendPrice([FromBody]ChangePricesData change)
        {
            Logger.InitLogger();

            try
            {
                if (change != null)
                {
                    Logger.Log.Debug("SendPrice: запуск с параметрами:\n" + JsonConvert.SerializeObject(change));
                    foreach (Price p in change.Prices)
                    {
                        Logger.Log.Debug("SendPrice: Отправка на пост: " + p);
                        SendPostResponse response = HttpSender.SendPost("http://109.196.164.28:5000/api/post/rate" , JsonConvert.SerializeObject(p));

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            Logger.Log.Error("SendPrice: " + String.Format("Ответ сервера: {0}\n{1}", response.StatusCode, response.Message) + Environment.NewLine);

                            return Request.CreateResponse(HttpStatusCode.Conflict);
                        }
                    }

                    Logger.Log.Debug("SendPrice: " + String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), "Отправлено успешно") + Environment.NewLine);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                Logger.Log.Error("SendPrice: change == null. Ошибка в данных запроса." + Environment.NewLine);
                var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);
                return responseBad;
            }
            catch (Exception e)
            {
                Logger.Log.Error("SendPrice: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);

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
                    Logger.Log.Debug("IncreaseBalace:  запуск с параметрами:\n" + JsonConvert.SerializeObject(balance));

                    string address = GetPostIp(balance.Post);

                    if(address != null || address != "")
                    {
                        SendPostResponse response = HttpSender.SendPost("http://" + address + "/api/post/balance/increase", JsonConvert.SerializeObject(balance));

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            Logger.Log.Error("IncreaseBalace: " + String.Format("Ответ сервера: {0}\n{1}", response.StatusCode, response.Message) + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.Conflict, "Не удалось установить связь с постом");
                        }

                        Logger.Log.Debug("IncreaseBalace: " + String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), "Отправлено успешно") + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        Logger.Log.Error("IncreaseBalance: не найден ip адрес поста" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Не найден ip адрес поста");
                    }
                    
                }
                Logger.Log.Error("IncreaseBalace: balance == null. Ошибка в данных запроса" + Environment.NewLine);
                var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);
                return responseBad;
            }
            catch (Exception e)
            {
                Logger.Log.Error("IncreaseBalace: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
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

                    string address = GetPostIp(post.Post);

                    if(address != null || address != "")
                    {
                        int balance = HttpSender.GetInt("http://"+ address + "/api/post/balance/get");
                        if (balance == -1)
                        {
                            Logger.Log.Error("Произошла ошибка при отправке запроса. Ответ -1" + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.Conflict, "Не удалось устаовить связь с постом");
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
                        Logger.Log.Error("GetCurrentBalance: не найден ip адрес поста" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Не найден ip адрес поста");
                    }
                }
                else
                {
                    Logger.Log.Error("GetCurrentBalance: Post == null. Ошибка в данных запроса" + Environment.NewLine);
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

                    string address = GetPostIp(post.Post);
                    if (address != null || address != "")
                    {
                        string result = HttpSender.GetString("http://"+ address + "/api/post/func/get");
                        PostFunction func = JsonConvert.DeserializeObject<PostFunction>(result);
                        if (func == null)
                        {
                            Logger.Log.Error("GetCurrentFunction: Произошла ошибка при отправке запроса. Ответ null" + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.Conflict, "Не удалось установить связь с постом");
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
                        Logger.Log.Error("GetCurrentFunction: не найден ip адрес поста" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Не найден ip адрес поста");
                    }
                    
                }
                else
                {
                    Logger.Log.Error("GetCurrentFunction: Post == null. Ошибка в данных запроса" + Environment.NewLine);
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
                if (func != null)
                {
                    Logger.Log.Debug(String.Format("SetFunction: Запуск с параметрами:\nPost: {0}, Function: {1}, Login: {2}", func.Post, func.Function, func.Login));

                    string address = GetPostIp(func.Post);
                    if (address != null || address != "")
                    {
                        SendPostResponse response = HttpSender.SendPost("http://"+ address + "/api/post/func/set", JsonConvert.SerializeObject(func));

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            Logger.Log.Error("SetFunction: " + String.Format("Ответ сервера: {0}\n{1}", response.StatusCode, response.Message) + Environment.NewLine);

                            return Request.CreateResponse(HttpStatusCode.Conflict);
                        }

                        Logger.Log.Debug("SetFunction: " + String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), "Отправлено успешно") + Environment.NewLine);

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        Logger.Log.Error("SetFunction: не найден ip адрес поста" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Не найден ip адрес поста");
                    }
                }
                else
                {
                    Logger.Log.Error("SetFunction: Function == null. Ошибка в данных запроса" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch(Exception ex)
            {
                Logger.Log.Error("SetFunction: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
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

                    string address = GetPostIp(post.Post);
                    if(address != null || address != "")
                    {
                        int heartbeat = HttpSender.GetInt("http://" + address + "/api/post/heartbeat");

                        if (heartbeat == -1)
                        {
                            Logger.Log.Error("HeartBeat: Произошла ошибка при отправке запроса. Ответ -1" + Environment.NewLine);
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
                        Logger.Log.Error("HeartBeat: не найден ip адрес поста" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Не найден ip адрес поста");
                    }
                }
                else
                {
                    Logger.Log.Error("HeartBeat: Post == null. Ошибка в данных запроса" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("HeartBeat: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private string GetPostIp(string code)
        {
            int? idDevice = _model.Posts.ToList().Find(x => x.Code == code).IDDevice;

            if (idDevice != null)
            {
                return _model.Device.ToList().Find(x => x.IDDevice == idDevice).IpAddress;
            }

            return null;
        }
    }
}
