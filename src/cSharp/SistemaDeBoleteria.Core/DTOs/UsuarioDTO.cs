using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Contraseña { get; set; }
    }
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
    }
    public class RegisterRequest
    {
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
    }

}