using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Inheritance;
using MySqlConnector;
using SistemaDeBoleteria.Core.Enums;
using Dapper;
using System.Data;
using System.Threading.Tasks.Dataflow;
using System.Data.Common;

namespace SistemaDeBoleteria.Repositories
{
    public class CodigoQRRepository :  DbRepositoryBase, ICodigoQRRepository
    {
        public CodigoQRRepository(string connectionString) : base (connectionString){}
        const string UpdSql = @"UPDATE QR 
                                SET TipoEstado = @Estado 
                                WHERE IdEntrada = @ID;";
        const string SlcEstado = @"SELECT TipoEstado 
                                   FROM QR 
                                   WHERE IdEntrada = @ID";
        const string InsSql = @"INSERT INTO QR (IdEntrada, Codigo)
                                VALUES (@ID, CONCAT('QR-', UUID()));";
        const string strUpdTrasEntradaAnulada = @"UPDATE QR
                                               SET TipoEstado = 'YaUsada'
                                               WHERE IdEntrada = @ID;";
        public CodigoQR? SelectById(int idEntrada) => UseNewConnection(db => db.QueryFirstOrDefault<CodigoQR>("SELECT * FROM QR WHERE IdEntrada = @ID", new { ID = idEntrada }));
        public ETipoEstadoQR UpdateEstado(int IdEntrada, ETipoEstadoQR estado) =>  UseNewConnection(db =>

            db.Execute(UpdSql, new { estado, ID = IdEntrada }) > 0 ?
            db.QueryFirstOrDefault<ETipoEstadoQR>(SlcEstado, new { ID = IdEntrada }) : ETipoEstadoQR.FirmaInvalida

        );
        public bool UpdAYaUsada(int idEntrada) => UseNewConnection(db => db.Execute(strUpdTrasEntradaAnulada, new { ID = idEntrada}) > 0);

        public bool Insert(int idEntrada) => UseNewConnection(db => db.Execute(InsSql, new{ ID = idEntrada}) > 0 ); 

        #region Validación de negocio
        const string SeltEntradaSql = @"SELECT Liquidez, Anulado 
                                        FROM Entrada 
                                        WHERE IdEntrada = @ID";
        const string SeltFuncionSql = @"SELECT F.Apertura, F.Cierre 
                                        FROM Orden O 
                                        JOIN Tarifa T USING (IdTarifa)
                                        JOIN Funcion F USING (IdFuncion) 
                                        WHERE O.IdOrden =  (SELECT IdOrden 
                                                            FROM Entrada 
                                                            WHERE IdEntrada = @ID)";
        const string SeltCodigQRSql = @"SELECT TipoEstado AS EstadoQR 
                                        FROM QR 
                                        WHERE IdEntrada = @ID";
        const string strExists = @"SELECT EXISTS(SELECT 1 
                                                 FROM QR 
                                                 WHERE IdEntrada = @ID
                                                 AND Codigo = @unCodigo)";
        public (Entrada, Funcion , CodigoQR) SelectData(int idEntrada) => UseNewConnection(db =>
        (
            db.QueryFirstOrDefault<Entrada>(SeltEntradaSql, new { ID = idEntrada}),
            db.QueryFirstOrDefault<Funcion>(SeltFuncionSql, new { ID = idEntrada}),
            db.QueryFirstOrDefault<CodigoQR>(SeltCodigQRSql, new { ID = idEntrada})
        ))!;
        public bool Exists(int idEntrada, string codigo) => UseNewConnection(db => db.ExecuteScalar<bool>( strExists, new { ID = idEntrada, unCodigo = codigo}));
        #endregion
    }
}