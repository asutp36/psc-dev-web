using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models.DTOs
{
    public class RoleInfoDto
    {
        public string Code { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public RoleInfoDto()
        {

        }

        /// <summary>
        /// Конструктор с кодом и названием
        /// </summary>
        /// <param name="code">Код роли</param>
        /// <param name="name">Отображаемое имя</param>
        public RoleInfoDto(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}
