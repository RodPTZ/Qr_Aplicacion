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
    public class CodigoQRAdo : ICodigoQRService
    {
        private readonly IDbConnection db;
        public CodigoQRAdo(string connectionString) => db = new MySqlConnection(connectionString);
        public CodigoQRAdo(IDbConnection dbConnection) => db = dbConnection;
        public CodigoQRAdo()
        {
            db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
        }

        public CodigoQR? GetQRByEntradaId(int idEntrada)
        {
            var sql = "SELECT * FROM QR WHERE IdEntrada = @ID";
            var codigoQR = db.QueryFirstOrDefault<CodigoQR>(sql, new { ID = idEntrada });
            return codigoQR;
        }
        public void ValidarQR(int IdEntrada)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@unIdEntrada", IdEntrada);
            db.Execute("ValidarQR", parameters);
        }

    }
}