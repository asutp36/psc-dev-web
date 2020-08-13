using CardsMobileService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Start() { }

        public void Stop() { }

        public bool IsExist(string post)
        {
            return true;
        }
    }
}
