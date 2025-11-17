using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface ILoginRepository
    {
        Usuario Insert(Usuario usuario);
        Usuario? Select(int idUsuario);
        Usuario? SelectMe(string email);
        bool UpdateRol(int idUsuario, string rol);
        Usuario? SelectByEmailAndPass(string Email, string Contraseña);
        bool Exists(int idUsuario);
    }
}