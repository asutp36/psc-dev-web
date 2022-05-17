using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "cwmonBackend"; // издатель токена
        public const string AUDIENCE = "cwmonClient"; // потребитель токена
        const string KEY = "no1body.can-hack!cwmon.Backend-matherf88ckers!!!1!";   // ключ для шифрации
        public const int LIFETIME = 300000; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
