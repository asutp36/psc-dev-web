using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using PostControllingService.Controllers.Supplies;
using PostControllingService.Models;

namespace PostControllingService.Controllers
{
    public class PostController : ApiController
    {
        ModelDb _model = new ModelDb();

        /// <summary>
        /// Запрос текущих тарифов по мойкам
        /// </summary>
        /// <param name="washes">Список кодов моек</param>
        /// <returns>Список режим/тариф</returns>
        /// <response code="200">Ок</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Ошибка во входных данных</response>
        [HttpPost]
        [ActionName("getrate")]
        public HttpResponseMessage GetCurrentRate([FromBody]string[] washes)
        {
            Logger.InitLogger();
            try
            {
                if (washes != null && washes.Length > 0)
                {
                    Logger.Log.Debug("GetCurrentRate: запуск с параметрами:\n" + JsonConvert.SerializeObject(washes));

                    List<WashRates> washRates = new List<WashRates>();
                    foreach (string washCode in washes)
                    {
                        List<Posts> posts = _model.Posts.Where(p => p.IDWash == _model.Wash.Where(w => w.Code == washCode).FirstOrDefault().IDWash && !p.Code.Contains("V")).ToList();

                        List<RatesWPostCode> postsRates = new List<RatesWPostCode>();
                        foreach (Posts p in posts)
                        {
                            string ip = GetPostIp(p.Code);
                            if (ip == null || ip.Equals(""))
                            {
                                Logger.Log.Error("GetCurrentRate: не найден ip адрес поста " + p.Code);
                                continue;
                            }
                               
                            HttpSenderResponse response = HttpSender.SendGet("http://" + ip + "/api/post/rate/get");

                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                Logger.Log.Error("GetCurrentRate: " + String.Format("Ответ сервера: {0}\n{1}", response.StatusCode, response.Message) + Environment.NewLine);
                                continue;
                            }

                            postsRates.Add(new RatesWPostCode
                            {
                                Post = p.Code,
                                Prices = JsonConvert.DeserializeObject<List<FunctionRate>>(response.Message)
                            });
                        }

                        washRates.Add(new WashRates
                        {
                            Wash = washCode,
                            Rates = postsRates
                        });
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(washRates));
                }
                else
                {
                    Logger.Log.Error("GetCurrentRate: posts == null. Ошибка в данных запроса" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error("GetCurrentRate: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Отправка новых тарифов на мойки
        /// </summary>
        /// <param name="change">Новые тарифы</param>
        /// <returns></returns>
        /// <response code="200">Ок</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Ошибка во входных данных</response>
        /// <response code="409">Код ответа поста был не 200</response>
        [HttpPost]
        [ActionName("rate")]
        public HttpResponseMessage SendRates([FromBody]ChangeRates change)
        {
            Logger.InitLogger();

            try
            {
                if (change != null)
                {
                    Logger.Log.Debug("SendRates: запуск с параметрами:\n" + JsonConvert.SerializeObject(change));

                    foreach(String washCode in change.Washes)
                    {
                        List<Posts> posts = _model.Posts.Where(p => p.IDWash == _model.Wash.Where(w => w.Code == washCode).FirstOrDefault().IDWash).ToList();

                        foreach (Posts p in posts)
                        {
                            Logger.Log.Debug("SendRates: Отправка на пост: " + p.Code);

                            HttpSenderResponse response = HttpSender.SendPost("http://" + GetPostIp(p.Code) + "/api/post/rate", JsonConvert.SerializeObject(change.Rates));

                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                Logger.Log.Error("SendRates: " + String.Format("Ответ сервера: {0}\n{1}", response.StatusCode, response.Message) + Environment.NewLine);

                                return Request.CreateResponse(HttpStatusCode.Conflict);
                            }
                        }
                    }

                    Logger.Log.Debug("SendRates: " + String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), "Отправлено успешно") + Environment.NewLine);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                Logger.Log.Error("SendRates: change == null. Ошибка в данных запроса." + Environment.NewLine);
                var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);
                return responseBad;
            }
            catch (Exception e)
            {
                Logger.Log.Error("SendRates: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Пополнить баланс поста
        /// </summary>
        /// <param name="balance"></param>
        /// <returns></returns>
        /// <response code="409">Код ответа поста был не 200</response>
        /// <response code="200">Ок</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Ошибка во входных данных</response>
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
                        HttpSenderResponse response = HttpSender.SendPost("http://" + address + "/api/post/balance/increase/site", JsonConvert.SerializeObject(balance));

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

        /// <summary>
        /// Узнать текущий баланс на посту
        /// </summary>
        /// <param name="post">Код поста</param>
        /// <returns>Balance в заголовке</returns>
        /// <response code="409">Код ответа поста был не 200</response>
        /// <response code="200">Ок, в заголовке Balance</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Ошибка во входных данных</response>
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
                        GetScalarResponse balanceResponse = HttpSender.GetScalar("http://" + address + "/api/post/balance/get");

                        if (balanceResponse.StatusCode != HttpStatusCode.OK)
                        {
                            Logger.Log.Error("GetBalance: Ошибка при запросе на пост: "+ balanceResponse.Result + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.Conflict, "Не удалось устаовить связь с постом");
                        }
                        else
                        { 
                            var response = Request.CreateResponse(HttpStatusCode.OK);
                            response.Headers.Add("Balance", balanceResponse.Result);
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

        /// <summary>
        /// Узнать текущую функцю на посте
        /// </summary>
        /// <param name="post">Код поста</param>
        /// <returns>Function в заголовке</returns>
        /// <response code="409">Код ответа поста был не 200</response>
        /// <response code="200">Ок, в заголовке Function</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Ошибка во входных данных</response>
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
                    if (address == null || address == "")
                    {
                        Logger.Log.Error("GetCurrentFunction: не найден ip адрес поста" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Не найден ip адрес поста");
                    }
                    else
                    {
                        GetScalarResponse getFuncResponse = HttpSender.GetScalar("http://" + address + "/api/post/func/get");
                        if (getFuncResponse.StatusCode != HttpStatusCode.OK && getFuncResponse.StatusCode != (HttpStatusCode)228)
                        {
                            Logger.Log.Error("GetCurrentFunction: Ошибка при запросе на пост: \n" + getFuncResponse.Result + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.Conflict, "Не удалось установить связь с постом");
                        }
                        else
                        {
                            Logger.Log.Debug("Перед строчкой 296");
                            if (getFuncResponse.StatusCode == (HttpStatusCode)228)
                            {
                                Logger.Log.Debug("После строчки 296");
                                Logger.Log.Debug("GetCurrentFunction: на посту функция = никакая" + Environment.NewLine);
                                return Request.CreateResponse(HttpStatusCode.OK, "");
                            }

                            PostFunction func = JsonConvert.DeserializeObject<PostFunction>(getFuncResponse.Result);
                            HttpResponseMessage response;
                            if (func.Name.Equals(""))
                                response = Request.CreateResponse(HttpStatusCode.OK, "");
                            else
                                response = Request.CreateResponse(HttpStatusCode.OK, func.Name);

                            Logger.Log.Debug("GetCurrentFunction: на посту функция = " + func.Name + Environment.NewLine);
                            response.Headers.Add("Function", func.Name);
                            return response;
                        }
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

        /// <summary>
        /// Задать текующую функцию на посту
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        /// <response code="409">Код ответа поста был не 200</response>
        /// <response code="200">Ок</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Ошибка во входных данных</response>
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
                        HttpSenderResponse response = HttpSender.SendPost("http://"+ address + "/api/post/func/set", JsonConvert.SerializeObject(func));

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

        /// <summary>
        /// Проверка связ с постом
        /// </summary>
        /// <param name="post">Код поста</param>
        /// <returns>Heartbeat в заголовках</returns>
        /// <response code="409">Код ответа поста был не 200</response>
        /// <response code="200">Ок, в заголовке Heartbeat</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Ошибка во входных данных</response>
        [HttpPost]
        [ActionName("heartbeat")]
        public HttpResponseMessage HeartBeat([FromBody]RequestWPostCode post)
        {
            Logger.InitLogger();

            try
            {
                if (post != null)
                {
                    Logger.Log.Debug("HeartBeat: PostCode: " + post.Post);

                    string address = GetPostIp(post.Post);
                    if(address != null || address != "")
                    {
                        GetScalarResponse heartbeatResponse = HttpSender.GetScalar("http://" + address + "/api/post/heartbeat");

                        if (heartbeatResponse.StatusCode == HttpStatusCode.OK)
                        {                            
                            var response = Request.CreateResponse(HttpStatusCode.OK);
                            response.Headers.Add("HeartBeat", heartbeatResponse.Result);
                            return response;                 
                        }
                        else
                        {
                            Logger.Log.Error("HeartBeat: Ошибка при запросе на пост:\n" + heartbeatResponse.Result + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.Conflict);
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

        [HttpGet]
        [ActionName("state")]
        public IHttpActionResult GetState(string postCode)
        {
            Logger.InitLogger();

            try
            {
                if(postCode == null)
                {
                    return BadRequest();
                }

                string ip = GetPostIp(postCode);
                if(ip == null)
                {
                    return NotFound();
                }

                GetScalarResponse response = HttpSender.GetScalar("http://" + ip + "/api/post/func/get");
                if (true) { }

                return Ok(ip);
            }
            catch(Exception e)
            {
                Logger.Log.Error("GetState: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        private string GetPostIp(string code)
        {
            Device device = _model.Device.Where(d => d.Code.Equals(code)).FirstOrDefault();
            if (device == null)
            {
                return null;
            }

            string ip = device.IpAddress;

            if (ip.Length > 1)
                return ip;

            return null;
        }

        private List<string> GetPostsIp(List<string> codes)
        {
            List<string> IPs = new List<string>();
            foreach (string code in codes)
                IPs.Add(GetPostIp(code));
            return IPs;
        }
    }
}
