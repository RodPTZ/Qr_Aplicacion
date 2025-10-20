using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Services
{
    public interface ILoginService
    {
        void AuthRegistrar(Usuario usuario);
        Usuario? AuthLogin(string nombreUsuario, string password);
        string AuthRefreshToken(string token);
        bool AuthLogout(string token);
        Usuario? AuthMe(string token);
        IEnumerable<string> GetRolesByUserId(int idUsuario);
        bool Auth_Assign_Change_Rol(int idUsuario, string rol);
    }
}