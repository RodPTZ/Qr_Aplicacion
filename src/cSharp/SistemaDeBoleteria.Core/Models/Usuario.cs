using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public RolUsuario Rol { get; set; }
        public string Token { get; set; }

        public Usuario(string nombreUsuario, string email, string contraseña, RolUsuario rol)
        {
            NombreUsuario = nombreUsuario;
            Email = email;
            Contraseña = contraseña;
            Rol = rol;
        }
        public Usuario()
        {
        }
        public enum RolUsuario
        {
            Admin,
            Empleado,
            Organizador,
            Cliente
        }
    }
}