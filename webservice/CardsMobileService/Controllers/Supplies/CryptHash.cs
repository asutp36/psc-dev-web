using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers.Supplies
{
    public class CryptHash
    {
        public static string GetHashCode(string str)
        {
            string salt = "$2a$07$30ydOQDXv5akDSajgDaSj19dSSKaGa2sdWE5Das94ds$";


            return BCrypt.Net.BCrypt.HashPassword(str, salt);
        }

        public static bool CheckHashCode(string hash, string basis)
        {
            return hash.Equals(GetHashCode(basis));
        }
    }
}
