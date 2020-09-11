using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies
{
    public class UserInfo
    {
        ModelDbContext _model = new ModelDbContext();
        public List<WashViewModel> GetWashes(List<Claim> claims)
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
                            name = w.Name
                        });
                    }
                }
            }

            return result;
        }
    }
}
