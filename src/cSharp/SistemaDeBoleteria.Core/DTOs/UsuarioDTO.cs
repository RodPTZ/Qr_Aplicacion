using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Enums;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Contraseña { get; set; }
    }
    public class LoginResponse
    {
        public string Email { get; set; }
        public string Rol { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class RegisterRequest
    {
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public ERolUsuario Rol { get; set; }
    }

    public class ViewMe
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
    }
}