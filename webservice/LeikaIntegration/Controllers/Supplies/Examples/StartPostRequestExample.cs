using LeikaIntegration.Controllers.Supplies;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeikaIntegration.Controllers
{
    public class StartPostRequestExample : IExamplesProvider<StartPostRequestBindingModel>
    {
        public StartPostRequestBindingModel GetExamples()
        {
            return new StartPostRequestBindingModel
            {
                dtime = "2020-08-07 12:27:00",
                hash = "$2a$07$30ydOQDXv5akDSajgDaSjubWyGrfbeTjI9BKwBU2kKtEdZd5O1.rC",
                post = "1111",
                amount = 100,
                clientID = ""
            };
        }
    }
}
