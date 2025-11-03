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
using System.Threading.Tasks.Dataflow;

namespace SistemaDeBoleteria.Repositories
{
    public class CodigoQRRepository :  DbRepositoryBase, ICodigoQRRepository
    {
        const string UpdSql = @"UPDATE QR 
                                SET TipoEstado = @Estado 
                                WHERE IdEntrada = @ID;";
        const string SeltEntradaSql = @"SELECT Liquidez, Estado AS EstadoEntrada 
                                        FROM Entrada WHERE IdEntrada = @ID";
        const string SeltFuncionSql = @"SELECT F.Apertura, F.Cierre 
                                        FROM Orden O 
                                        JOIN Funcion F USING (IdFuncion) 
                                        WHERE O.IdOrden =  (SELECT IdOrden 
                                                            FROM Entrada 
                                                            WHERE IdEntrada = @ID)";
        const string SeltCodigQRSql = @"SELECT TipoEstado AS EstadoQR 
                                        FROM QR 
                                        WHERE IdEntrada = @ID";
        
        public CodigoQR? SelectById(int idEntrada) => db.QueryFirstOrDefault<CodigoQR>("SELECT * FROM QR WHERE IdEntrada = @ID", new { ID = idEntrada });
        public CodigoQR.estadoQR UpdateEstado(int IdEntrada, CodigoQR.estadoQR estado)
        {

            db.Execute(UpdSql, new { estado, ID = IdEntrada });
            // if (codigoQR == null)
            // {
            //     throw new InvalidOperationException("No se encontró el código QR para la entrada especificada.");
            // }
            var QR = db.QueryFirstOrDefault<CodigoQR>("SELECT * FROM QR WHERE IdEntrada = @ID", new { ID = IdEntrada });
             if (QR == null)
            {
                throw new InvalidOperationException("Este QR tambien es null ");
            }
            return QR.TipoEstado; ;
        }
        public (Entrada entrada, Funcion funcion, CodigoQR codigoQR) SelectData(int idEntrada)
        {
            var DataEntrada = db.QueryFirstOrDefault<Entrada>(SeltEntradaSql, new { ID = idEntrada});
            var DataFuncion = db.QueryFirstOrDefault<Funcion>(SeltFuncionSql, new { ID = idEntrada});
            var DataQR = db.QueryFirstOrDefault<CodigoQR>(SeltCodigQRSql, new { ID = idEntrada});

            return (DataEntrada, DataFuncion, DataQR)!;
        }
        public bool Exists(string codigo)
        {
            var exists =db.Query<CodigoQR>("SELECT * FROM QR WHERE Codigo = @unCodigo", new { unCodigo = codigo}).Any();
            return exists;
        }

    }
}