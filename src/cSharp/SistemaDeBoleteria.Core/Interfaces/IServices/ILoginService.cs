using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Interfaces.IServices
{
    public interface ILoginService
    {
        Usuario Register(RegisterRequest registerRequest);
        LoginResponse Login(LoginRequest loginRequest);
        string RefreshToken(string token);
        bool Logout(string token);
        Usuario? Me(string token);
        bool ChangeRol(int idUsuario, string rol);
    }
}