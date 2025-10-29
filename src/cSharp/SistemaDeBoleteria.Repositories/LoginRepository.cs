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

namespace SistemaDeBoleteria.Repositories
{
    public class LoginRepository : DbRepositoryBase, ILoginRepository
    {
        const string InsSql = @"INSERT INTO Usuario (NombreUsuario, Email, Contraseña) 
                                    VALUES (@NombreUsuario, @Email, @Contraseña);

                                    SELECT LAST_INSERT_ID();";
        public Usuario Insert(Usuario usuario)
        {
            usuario.IdUsuario = db.ExecuteScalar<int>(InsSql, usuario);
            return usuario;
        }

        public Usuario? Get(string nombreUsuario, string password)
        {
            var sql = "SELECT * FROM Usuario WHERE Email = @Email AND Clave = @Clave";
            var usuario = db.QueryFirstOrDefault<Usuario>(sql, new { Email = nombreUsuario, Clave = password });
            return usuario;
        }

        public Usuario? GetMe(string token)
        {
            // lógica para obtener el usuario actual
            // var sql = "SELECT * FROM Usuario WHERE Email = @email AND Clave = @clave";
            // var usuarioEncontrado = db.QueryFirstOrDefault(sql, new
            // {
            //     usuario.Email,
            //     usuario.Clave
            // });
            // return usuarioEncontrado!;
            return null;
        }
        public bool UpdateRol(int idUsuario, string rol)
        {
            // lógica para asignar o cambiar el rol del usuario
            return true;
        }
    }
}