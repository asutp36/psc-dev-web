using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models.DTOs
{
    public class RoleDTO
    {
        public int IdRole { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public RoleDTO()
        {

        }

        /// <summary>
        /// Конструктор с кодом и названием
        /// </summary>
        /// <param name="code">Код роли</param>
        /// <param name="name">Отображаемое имя</param>
        public RoleDTO(int idRole, string code, string name)
        {
            IdRole = IdRole;
            Code = code;
            Name = name;
        }
    }
}
