using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.Inheritance;
using System.Security.Cryptography;

namespace SistemaDeBoleteria.Repositories
{
    public class LoginRepository : DbRepositoryBase, ILoginRepository
    {
        public LoginRepository(string connectionString) : base (connectionString){}
        const string InsSql = @"INSERT INTO Usuario (NombreUsuario, Email, Contraseña, Rol) 
                                VALUES (@NombreUsuario, @Email, SHA2(@Contraseña, 256), @Rol);

                                SELECT LAST_INSERT_ID();";
        const string UpdRol = @"UPDATE Usuario 
                                SET Rol = @Rol 
                                WHERE IdUsuario = @IdUsuario;";
        public Usuario Insert(Usuario usuario) => UseNewConnection(db =>
        {
            usuario.IdUsuario = db.ExecuteScalar<int>(InsSql, usuario);
            return usuario;
        });

        public Usuario? Select(int idUsuario) => UseNewConnection(db => db.QueryFirstOrDefault<Usuario>("SELECT * FROM Usuario WHERE IdUsuario = @ID;", new { ID = idUsuario }));
        public Usuario? SelectMe(string email) => UseNewConnection(db => db.QueryFirstOrDefault<Usuario>("SELECT * FROM Usuario WHERE Email = @Email;", new { Email = email }));
        public bool UpdateRol(int idUsuario, string rol) => UseNewConnection(db => db.ExecuteScalar<bool>(UpdRol, new { Rol = rol, IdUsuario = idUsuario }));

        #region Validación de negocio
        const string strSBEAP = @"SELECT * 
                                  FROM Usuario 
                                  WHERE Email = @Email 
                                  AND Contraseña = SHA2(@Contraseña, 256)";
        const string strExists = @"SELECT EXISTS(SELECT 1 
                                                 FROM Usuario 
                                                 WHERE IdUsuario = @ID)";
        public Usuario? SelectByEmailAndPass(string Email, string Contraseña) 
        => UseNewConnection(db => db.QueryFirstOrDefault<Usuario>(strSBEAP, new { Email, Contraseña }));
        public bool Exists(int idUsuario) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new { ID = idUsuario}));
        #endregion
    }
}