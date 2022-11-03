using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models.DTOs
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public AccessLevel Eco { get; set; }
        public AccessLevel GateWash { get; set; }
        public bool RefillGateWash { get; set; }

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
            Id = idRole;
            Code = code;
            Name = name;
        }
    }
}
