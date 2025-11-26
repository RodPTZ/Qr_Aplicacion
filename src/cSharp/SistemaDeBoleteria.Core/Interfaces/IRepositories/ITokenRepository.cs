using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface ITokenRepository
    {
        bool InsertToken(int idUsuario, string token, DateTime expiracion);
        bool InvalidateToken(string token);
        (bool Revocado, DateTime Expiración) IsRevoced(string token);
        Usuario? SelectUserByToken(string refreshToken);
    }
}