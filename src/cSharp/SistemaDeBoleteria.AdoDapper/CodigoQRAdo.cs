using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Services;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using QRCoder;

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

        public byte[] GetQRByEntradaId(int idEntrada)
        {
            var sql = "SELECT * FROM QR WHERE IdEntrada = @ID";
            var codigoQR = db.QueryFirstOrDefault<CodigoQR>(sql, new { ID = idEntrada });
            if (codigoQR is null)
            {
                return null!;
            }
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(codigoQR.Codigo, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new SvgQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            byte[] qrCodeBytes = System.Text.Encoding.UTF8.GetBytes(qrCodeImage);
            return qrCodeBytes;
        }
        public string ValidarQR(int IdEntrada)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@unIdEntrada", IdEntrada);
            db.Execute("ValidarQR", parameters);
            var sql = "SELECT TipoEstado FROM QR WHERE IdEntrada = @ID";
            var estado = db.QueryFirstOrDefault<string>(sql, new { ID = IdEntrada });
            return estado!;

        }

    }
}