using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Inheritance;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;

namespace SistemaDeBoleteria.Repositories
{
    public class TokenRepository : DbRepositoryBase, ITokenRepository
    {
        public TokenRepository(string connectionString) : base (connectionString){}
        const string InsTkn = @"INSERT INTO AuthTokens (IdUsuario, Token, Expiracion) 
                                VALUES (@IdUsuario, @Token, @Expiracion);";
        const string InvTkn = @"UPDATE AuthTokens 
                                SET Expiracion = NOW(), 
                                    Revocado = TRUE 
                                WHERE Token = @Token;";
        const string SlcUsr = @"SELECT U.* 
                                FROM AuthTokens T
                                JOIN Usuario U ON T.IdUsuario = U.IdUsuario
                                WHERE T.Token = @Token 
                                AND T.Expiracion > NOW() 
                                AND T.Revocado = FALSE;";
        const string SlcData = @"SELECT Revocado, Expiracion 
                                 FROM AuthTokens 
                                 WHERE Token = @Token;";
        public bool InsertToken(int idUsuario, string refreshToken, DateTime expiracion) 
        => UseNewConnection(db => db.ExecuteScalar<bool>(InsTkn, new { idUsuario, Token = refreshToken, expiracion }));
        public bool InvalidateToken(string token) => UseNewConnection(db =>
        {
            var rowsAffected = db.Execute(InvTkn, new { Token = token });
            return rowsAffected > 0;
        });
        public (bool Revocado, DateTime Expiración) IsRevoced(string token)
        => UseNewConnection(db => db.QueryFirstOrDefault<(bool, DateTime)>(SlcData, new { Token = token }));

        public Usuario? SelectUserByToken(string refreshToken) => UseNewConnection(db => db.QueryFirstOrDefault<Usuario>(SlcUsr, new { Token = refreshToken }));
    }
}