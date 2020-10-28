using Backend.Controllers.Supplies.Filters;
using Backend.Models;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies
{
    public class UserInfo
    {
        private ModelDbContext _model = new ModelDbContext();
        public List<Claim> claims { get; set; }
        private List<WashViewModel> _washes;


        public UserInfo(List<Claim> c)
        {
            this.claims = c;
        }

        public List<WashViewModel> GetWashes()
        {
            List<WashViewModel> result = new List<WashViewModel>();

            foreach (Claim c in claims)
            {
                if (c.Type == ClaimsIdentity.DefaultRoleClaimType)
                {
                    List<RoleWash> availableWashes = _model.RoleWash.Where(rw => rw.Idrole.Equals(_model.Roles.Where(r => r.Code.Equals(c.Value)).FirstOrDefault().Idrole)).ToList();
                    foreach (RoleWash rw in availableWashes)
                    {
                        Wash w = _model.Wash.Find(rw.Idwash);
                        result.Add(new WashViewModel()
                        {
                            idWash = w.Idwash,
                            code = w.Code,
                            name = w.Name,
                            idRegion = w.Idregion
                        });
                    }
                }
            }
            _washes = result;
            return result;
        }

        public List<PostViewModel> GetPosts()
        {
            if (_washes == null)
                this.GetWashes();
            List<PostViewModel> result = new List<PostViewModel>();

            foreach (WashViewModel w in _washes)
            {
                List<Posts> posts = _model.Posts.Where(p => p.Idwash.Equals(w.idWash)).Include(p => p.IddeviceNavigation).ToList();

                foreach (Posts p in posts)
                {
                    if (p.Iddevice == null)
                        continue;

                    result.Add(new PostViewModel()
                    {
                        code = p.IddeviceNavigation.Code,
                        name = p.IddeviceNavigation.Name,
                        idWash = w.idWash
                    });
                }
            }
            return result;
        }

        public List<RegionViewModel> GetRegions()
        {
            if (_washes == null)
                this.GetWashes();
            List<RegionViewModel> result = new List<RegionViewModel>();

            foreach (WashViewModel w in _washes)
            {
                Regions region = _model.Regions.Find(w.idRegion);

                RegionViewModel rvm = new RegionViewModel()
                {
                    idRegion = region.Idregion,
                    code = region.Code,
                    name = region.Name
                };

                if (result.Find(r => r.idRegion == rvm.idRegion) == null)
                    result.Add(rvm);
            }
            return result;
        }

        public List<ChangerViewModel> GetChangers()
        {
            if (_washes == null)
                this.GetWashes();

            List<ChangerViewModel> result = new List<ChangerViewModel>();

            foreach (WashViewModel w in _washes)
            {
                List<Changers> changers = _model.Changers.Where(c => c.Idwash == w.idWash).Include(c => c.IddeviceNavigation).ToList();

                foreach (Changers c in changers)
                {
                    if (c.Iddevice == null)
                        continue;

                    result.Add(new ChangerViewModel { code = c.IddeviceNavigation.Code, name = c.IddeviceNavigation.Name, idRegion = w.idRegion });
                }
            }

            return result;
        }

        public List<OperationTypeViewModel> GetOperationTypes()
        {
            List<OperationTypeViewModel> result = new List<OperationTypeViewModel>();

            List<OperationTypes> types = _model.OperationTypes.ToList();
            foreach(OperationTypes ot in types)
            {
                result.Add(new OperationTypeViewModel { code = ot.Code, name = ot.Name });
            }

            return result;
        }
    }
}
