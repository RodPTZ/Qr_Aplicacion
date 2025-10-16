using SistemaDeBoleteria.Core.Services;
using SistemaDeBoleteria.Core;
using MySqlConnector;
using Dapper;
using System.Data;
namespace SistemaDeBoleteria.AdoDapper;

public class FuncionAdo : IFuncionService
{
    private readonly IDbConnection db;
    public FuncionAdo(string connectionString) => db = new MySqlConnection(connectionString);
    public FuncionAdo(IDbConnection dbConnection) => db = dbConnection;
    public FuncionAdo()
    {
        db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
    }

    public void InsertFuncion(Funcion funcion)
        {
            var sql = "INSERT INTO Funcion (IdEvento, IdSector, IdSesion,  Fecha, Duracion, cancelado) VALUES (@IdEvento, @IdSector, @IdSesion, @Duracion, @Fecha, @Cancelado);";
            var id = db.ExecuteScalar<int>(sql, new
            {
                funcion.IdEvento,
                funcion.IdSector,
                funcion.IdSesion,
                funcion.Fecha,
                funcion.Duración,
                funcion.cancelado
            });
            funcion.IdFuncion = id;
        }

        public IEnumerable<Funcion> GetFunciones()
        {
            var sql = "SELECT * FROM Funcion";
            return db.Query<Funcion>(sql);
        }

        public Funcion? GetFuncionById(int IdFuncion)
        {
            var sql = "SELECT * FROM Funcion WHERE IdFuncion = @ID";
            return db.QueryFirstOrDefault<Funcion>(sql, new { ID = IdFuncion });
        }

        public void UpdateFuncion(Funcion funcion, int IdFuncion)
        {
            var sql = "UPDATE Funcion SET IdSector = @IdSector, IdSesion = @IdSesion, Fecha = @Fecha, Duracion = @Duracion WHERE IdFuncion = @ID";
            db.Execute(sql, new
            {
                funcion.IdSector,
                funcion.IdSesion,
                funcion.Fecha,
                funcion.Duración,
                ID = IdFuncion
            });
        }
        public void CancelarFuncion(int IdFuncion)
        {
            var sql = "UPDATE Funcion SET cancelado = true WHERE IdFuncion = @ID";
            db.Execute(sql, new { ID = IdFuncion });
        }

}
