using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Services;
using SistemaDeBoleteria.Core;
using MySqlConnector;
using Dapper;
using System.Data;

namespace SistemaDeBoleteria.AdoDapper
{
    public class AuthAdo : ILoginService
    {
        private readonly IDbConnection db;
        public AuthAdo(string connectionString) => db = new MySqlConnection(connectionString);
        public AuthAdo(IDbConnection dbConnection) => db = dbConnection;
        public AuthAdo()
        {
            db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
        }
        public void AuthRegistrar(Usuario usuario)
        {
            var sql = "INSERT INTO Usuario( Email, Clave) VALUES (@Email, @Clave)";
            db.Execute(sql, new
            {
                usuario.Email,
                usuario.Clave
            });
        }

        public Usuario? AuthLogin(string nombreUsuario, string password)
        {
            var sql = "SELECT * FROM Usuario WHERE Email = @Email AND Clave = @Clave";
            var usuario = db.QueryFirstOrDefault<Usuario>(sql, new { Email = nombreUsuario, Clave = password });
            return usuario;
        }
        public string AuthRefreshToken(string token)
        {
            // lógica para refrescar el token
            return token;
        }
        public bool AuthLogout(string token)
        {
            // lógica para cerrar sesión
            return true;
        }
        public Usuario? AuthMe(string token)
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
        public IEnumerable<string> GetRolesByUserId(int idUsuario)
        {
            // lógica para obtener los roles del usuario
            return Enumerable.Empty<string>();
        }
        public bool Auth_Assign_Change_Rol(int idUsuario, string rol)
        {
            // lógica para asignar o cambiar el rol del usuario
            return true;
        }
    }
}