using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Enums;
namespace SistemaDeBoleteria.Core.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public ERolUsuario Rol { get; set; }

        public Usuario(string nombreUsuario, string email, string contraseña, ERolUsuario rol)
        {
            NombreUsuario = nombreUsuario;
            Email = email;
            Contraseña = contraseña;
            Rol = rol;
        }
        public Usuario()
        {
        }
    }
}