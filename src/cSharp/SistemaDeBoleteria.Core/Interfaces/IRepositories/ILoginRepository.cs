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
        Usuario? Get(string nombreUsuario, string password);
        Usuario? GetMe(string token);
        bool UpdateRol(int idUsuario, string rol);
    }
}