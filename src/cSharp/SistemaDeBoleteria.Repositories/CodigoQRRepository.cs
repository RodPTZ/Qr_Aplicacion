using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Inheritance;
using MySqlConnector;
using Dapper;
using System.Data;
using QRCoder;

namespace SistemaDeBoleteria.Repositories
{
    public class CodigoQRRepository :  DbRepositoryBase, ICodigoQRRepository
    {
        private readonly string UpdEstado =@"UPDATE QR SET TipoEstado = @Estado WHERE IdEntrada = @ID;";
        public CodigoQR? SelectById(int idEntrada) => db.QueryFirstOrDefault<CodigoQR>("SELECT * FROM QR WHERE IdEntrada = @ID", new { ID = idEntrada });
        public string UpdateEstado(int IdEntrada, string estado)
        {
            // var parameters = new DynamicParameters();
            // parameters.Add("@unIdEntrada", IdEntrada);
            // db.Execute("ValidarQR", parameters);
            // var sql = "SELECT TipoEstado FROM QR WHERE IdEntrada = @ID";
            // var estado = db.QueryFirstOrDefault<string>(sql, new { ID = IdEntrada });
            // return estado!;
            var codigo = db.ExecuteScalar<string>(UpdEstado, new { estado, ID = IdEntrada} );
            return codigo!;
        }

    }
}