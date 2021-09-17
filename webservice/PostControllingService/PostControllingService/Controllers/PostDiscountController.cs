using Newtonsoft.Json;
using PostControllingService.Controllers.Supplies;
using PostControllingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PostControllingService.Controllers
{
    public class PostDiscountController : ApiController
    {
        ModelDb _model = new ModelDb();

        [HttpGet]
        public IHttpActionResult Get([FromUri]string washCode)
        {
            Logger.InitLogger();
            try
            {
                if (washCode == null || washCode == "")
                {
                    Logger.Log.Error("PostDiscount.Get: нет кода мойки" + Environment.NewLine);
                    return BadRequest();
                }

                List<PostDiscount> result = new List<PostDiscount>();
                List<Posts> posts = _model.Posts.Where(p => p.IDWash == _model.Wash.Where(w => w.Code == washCode).FirstOrDefault().IDWash && !p.Code.Contains("V")).ToList();
                foreach(Posts p in posts)
                {
                    string ip = GetPostIp(p.Code);
                    if (ip == null || ip.Equals(""))
                    {
                        Logger.Log.Error("PostDiscount.Get: не найден ip адрес поста " + p.Code);
                        continue;
                    }

                    HttpSenderResponse response = HttpSender.SendGet("http://" + ip + "api/post/get/happyhours");

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Log.Error("PostDiscount.Get: " + String.Format("Ответ поста: {0}\n{1}", response.StatusCode, response.ResultMessage) + Environment.NewLine);
                        continue;
                    }

                    PostDiscount discount = JsonConvert.DeserializeObject<PostDiscount>(response.ResultMessage);
                    discount.Post = p.Code;
                    result.Add(discount);
                }

                return Ok(result);
            }
            catch(Exception e)
            {
                Logger.Log.Error("PostDiscount.Get: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult Set([FromBody]PostDiscount model)
        {
            Logger.InitLogger();

            try
            {
                if(!ModelState.IsValid)
                {
                    Logger.Log.Error("PostDiscount.Set: входные параметры некорректные: " + JsonConvert.SerializeObject(model) + Environment.NewLine);
                    return BadRequest();
                }

                string postIp = GetPostIp(model.Post);
                if(postIp == null || postIp == "")
                {
                    Logger.Log.Error($"PostDiscount.Set: не найден ip поста {model.Post}" + Environment.NewLine);
                    return NotFound();
                }

                HttpSenderResponse response = HttpSender.SendPost("http://" + postIp + "api/post/set/happyhours", JsonConvert.SerializeObject(model));

                return StatusCode(response.StatusCode);
            }
            catch(Exception e)
            {
                Logger.Log.Error("PostDiscount.Get: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return InternalServerError();
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
    }
}
