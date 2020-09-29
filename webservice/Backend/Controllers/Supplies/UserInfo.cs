using Backend.Models;
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

            return result;
        }

        public List<PostViewModel> GetPosts()
        {
            List<WashViewModel> washes = this.GetWashes();
            List<PostViewModel> result = new List<PostViewModel>();

            foreach (WashViewModel w in washes)
            {
                List<Posts> posts = _model.Posts.Where(p => p.Idwash.Equals(w.idWash)).Include(p => p.IddeviceNavigation).ToList();

                foreach(Posts p in posts)
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
            List<WashViewModel> washes = this.GetWashes();
            List<RegionViewModel> result = new List<RegionViewModel>();

            foreach (WashViewModel w in washes)
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
    }
}
