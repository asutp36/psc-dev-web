using CardsMobileService.Controllers.Supplies;
using CardsMobileService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers
{
    public class PostApi
    {
        private ModelDbContext _model = new ModelDbContext();
        ILogger<PostApi> _logger;

        public PostApi(ILogger<PostApi> logger)
        {
            _logger = logger;
        }

        public string Start(PostActionModel model) 
        {
            if(model.amount < 50)
            {
                return "weak";
            }

            HttpResponse response = HttpSender.SendPost("http://" + GetIp(model.post) + "/api/post/balance/increase/card", 
                JsonConvert.SerializeObject(new StartModel
            {
                cardNum = model.cardNum,
                amount = model.amount
            }));

            if(response.StatusCode == 0)
            {
                return "unavailible";
            }

            if(response.StatusCode == (HttpStatusCode)423)
            {
                return "busy";
            }

            if(response.StatusCode == HttpStatusCode.OK)
            {
                return "ok";
            }

            return JsonConvert.SerializeObject(response);

        }

        public void Stop() { }

        public string GetIp(string post) 
        {
            return _model.Device.Where(d => d.Code.Equals(post)).FirstOrDefault().IpAddress;
        }

        public bool IsExist(string post)
        {
            return _model.Device.Where(d => d.Code.Equals(post)).ToList().Count > 0;
        }

        public string GetWashCode(string post)
        {
            int idDevice = _model.Device.Where(d => d.Code.Equals(post)).FirstOrDefault().Iddevice;

            int idWash = _model.Posts.Where(p => p.Iddevice == idDevice).FirstOrDefault().Idwash;

            return _model.Wash.Find(idWash).Code;
        }
    }
}
