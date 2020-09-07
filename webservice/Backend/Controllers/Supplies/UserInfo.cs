using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies
{
    public class UserInfo
    {
        public static List<string> GetWashes(List<Claim> claims)
        {
            List<string> result = new List<string>();

            foreach (Claim c in claims)
            {
                if (c.Type == ClaimsIdentity.DefaultRoleClaimType)
                    result.Add(c.Value);
            }

            return result;
        }
    }
}
