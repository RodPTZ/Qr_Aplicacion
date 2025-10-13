using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace SistemaDeBoleteria.AdoDapper
{
    public class Dapper
    {
        private readonly IDbConnection db;
        public Dapper(string connectionString) => db = new MySqlConnection(connectionString);
        public Dapper(IDbConnection dbConnection) => db = dbConnection;
        public Dapper()
        {
            db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
        }

        #region Locales y Sectores
        public IEnumerable<Local> GetLocales()
        {
            var sql = "SELECT * FROM Local";
            return db.Query<Local>(sql);
        }
        public Local? GetLocalById(int id)
        {
            var sql = "SELECT * FROM Local WHERE IdLocal = @ID";
            return db.QueryFirstOrDefault<Local>(sql, new { ID = id });
        }
        public void InsertLocal(Local local)
        {
            var sql = "INSERT INTO Local (Nombre, Ubicacion) VALUES (@Nombre, @Ubicacion);";
            var id = db.ExecuteScalar<int>(sql, local);
            local.IdLocal = id;
        }
        public void UpdateLocal(Local local)
        {
            var sql = "UPDATE Local SET Nombre = @Nombre, Ubicacion = @Ubicacion WHERE IdLocal = @ID";
            db.Execute(sql, local);
        }
        public bool DeleteLocal(int id)
        {
            var consulta = "SELECT COUNT(*) FROM Funcion WHERE IdLocal = @ID";
            var cantidadFunciones = db.ExecuteScalar<int>(consulta, new { ID = id });
            if (cantidadFunciones == 0)
            {
                var sql = "DELETE FROM Local WHERE IdLocal = @ID";
                db.Execute(sql, new { ID = id });
                return true;
            }
            return false;
        }

        public void InsertSector(Sector sector, int idLocal)
        {

            var sql = "INSERT INTO Sector (IdLocal, TipoSector) VALUES (@IdLocal, @TipoSector);";
            var id = db.ExecuteScalar<int>(sql, new
            {
                idLocal,
                sector.TipoSector
            });
            sector.IdSector = id;
        }

        public IEnumerable<Sector> GetSectoresByLocalId(int idLocal)
        {
            var sql = "SELECT * FROM Sector WHERE IdLocal = @ID";
            return db.Query<Sector>(sql, new { ID = idLocal });
        }
        public Sector? GetSectorById(int id)
        {
            var sql = "SELECT * FROM Sector WHERE IdSector = @ID";
            return db.QueryFirstOrDefault<Sector>(sql, new { ID = id });
        }
        public bool UpdateSector(Sector sector, int id)
        {
            var sql = "UPDATE Sector SET TipoSector = @TipoSector WHERE IdSector = @ID";
            db.Execute(sql, new
            {
                sector.TipoSector,
                ID = id
            });
            return true;
        }
        public bool DeleteSector(int id)
        {
            var consulta = "SELECT COUNT(*) FROM Funcion WHERE IdSector = @ID";
            var cantidadFunciones = db.ExecuteScalar<int>(consulta, new { ID = id });
            if (cantidadFunciones == 0)
            {
                var sql = "DELETE FROM Sector WHERE IdSector = @ID";
                db.Execute(sql, new { ID = id });
                return true;
            }
            return false;
        }
        #endregion

        #region Eventos

        public IEnumerable<Evento> GetEventos()
        {
            var sql = "SELECT * FROM Evento";
            return db.Query<Evento>(sql);
        }

        public Evento? GetEventoById(int id)
        {
            var sql = "SELECT * FROM Evento WHERE IdEvento = @ID";
            return db.QueryFirstOrDefault<Evento>(sql, new { ID = id });
        }

        public void InsertEvento(Evento evento)
        {
            var sql = "INSERT INTO Evento (IdLocal, Nombre, Tipo, publicado) VALUES (@IdLocal, @Nombre, @Tipo, @Publicado);";
            var id = db.ExecuteScalar<int>(sql, new
            {
                evento.IdLocal,
                evento.Nombre,
                Tipo = evento.Tipo.ToString(),
                evento.publicado
            });
            evento.IdEvento = id;
        }
        public void UpdateEvento(Evento evento)
        {
            var sql = "UPDATE Evento SET IdLocal = @IdLocal, Nombre = @Nombre, Tipo = @Tipo, publicado = @Publicado WHERE IdEvento = @ID";
            db.Execute(sql, new
            {
                evento.IdLocal,
                evento.Nombre,
                Tipo = evento.Tipo.ToString(),
                evento.publicado,
                ID = evento.IdEvento
            });
        }
        public bool PublicarEvento(int id)
        {
            var consulta = "SELECT COUNT(*) FROM Funcion WHERE IdEvento = @ID";
            var cantidadFunciones = db.ExecuteScalar<int>(consulta, new { ID = id });
            if (cantidadFunciones == 0)
                return false;
            var sql = "UPDATE Evento SET publicado = true WHERE IdEvento = @ID";
            db.Execute(sql, new { ID = id });
            return true;
        }
        public bool CancelarEvento(int id)
        {
            var sql = "UPDATE Evento SET publicado = false WHERE IdEvento = @ID";
            db.Execute(sql, new { ID = id });
            return true;
        }
        #endregion

        #region Funciones

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

        public Funcion? GetFuncionById(int id)
        {
            var sql = "SELECT * FROM Funcion WHERE IdFuncion = @ID";
            return db.QueryFirstOrDefault<Funcion>(sql, new { ID = id });
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
        public void CancelarFuncion(int id)
        {
            var sql = "UPDATE Funcion SET cancelado = true WHERE IdFuncion = @ID";
            db.Execute(sql, new { ID = id });
        }

        #endregion

        #region Tarifas y Stock

        public void InsertTarifa(Tarifa tarifa)
        {
            var sql = "INSERT INTO Tarifa (IdFuncion, Precio, Stock) VALUES (@IdFuncion, @Precio, @Stock);";
            var id = db.ExecuteScalar<int>(sql, tarifa);
            tarifa.IdTarifa = id;
        }
        public IEnumerable<Tarifa> GetTarifasByFuncionId(int idFuncion)
        {
            var sql = "SELECT * FROM Tarifa WHERE IdFuncion = @ID";
            return db.Query<Tarifa>(sql, new { ID = idFuncion });
        }
        public Tarifa? GetTarifaById(int id)
        {
            var sql = "SELECT * FROM Tarifa WHERE IdTarifa = @ID";
            return db.QueryFirstOrDefault<Tarifa>(sql, new { ID = id });
        }
        public void UpdateTarifa(Tarifa tarifa, int IdTarifa)
        {
            var sql = "UPDATE Tarifa SET Precio = @Precio, Stock = @Stock, Estado = @Estado WHERE IdTarifa = @ID";
            db.Execute(sql, new
            {
                tarifa.precio,
                tarifa.stock,
                tarifa.estado,
                ID = IdTarifa
            });
        }

        #endregion

        #region Clientes
        public IEnumerable<Cliente> GetClientes()
        {
            var sql = "SELECT * FROM Cliente";
            return db.Query<Cliente>(sql);
        }
        public Cliente? GetClienteById(int id)
        {
            var sql = "SELECT * FROM Cliente WHERE IdCliente = @ID";
            return db.QueryFirstOrDefault<Cliente>(sql, new { ID = id });
        }
        public void InsertCliente(Cliente cliente)
        {
            var sql = "INSERT INTO Cliente (Nombre, Apellido, DNI, Email, Telefono, Localidad, Edad) VALUES (@Nombre, @Apellido, @DNI, @Email, @Telefono, @Localidad, @Edad);";
            var id = db.ExecuteScalar<int>(sql, cliente);
            cliente.IdCliente = id;
        }
        public void UpdateCliente(Cliente cliente)
        {
            var sql = "UPDATE Cliente SET Nombre = @Nombre, Apellido = @Apellido, DNI = @DNI, Email = @Email, Telefono = @Telefono, Localidad = @Localidad WHERE IdCliente = @IdCliente";
            db.Execute(sql, cliente);
        }
        #endregion

        #region Ordenes

        public IEnumerable<Orden> GetOrdenes()
        {
            var sql = "SELECT * FROM Orden";
            return db.Query<Orden>(sql);
        }
        public Orden? GetOrdenById(int id)
        {
            var sql = "SELECT * FROM Orden WHERE IdOrden = @ID";
            return db.QueryFirstOrDefault<Orden>(sql, new { ID = id });
        }
        public void InsertOrden(Orden orden)
        {
            var sql = "INSERT INTO Orden (IdCliente, IdSesion, TipoEntrada, MedioDePago, Emision, Cierre, Abonado, Cancelado) VALUES (@IdCliente, @IdSesion, @TipoEntrada, @MedioDePago, NOW(),  DATE_ADD(NOW(), INTERVAL 15 MINUTE));";
            var id = db.ExecuteScalar<int>(sql, new
            {
                orden.cliente?.IdCliente,
                orden.IdSesion,
                TipoEntrada = orden.tipoEntrada.ToString(),
                orden.MedioDePago
            });
            orden.IdOrden = id;
        }
        public void PagarOrden(int id)
        {
            var sql = "UPDATE Orden SET Abonado = true WHERE IdOrden = @ID";
            db.Execute(sql, new { ID = id });
            var insEntrada = "INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez) VALUES (@ID,(SELECT TipoEntrada FROM Orden WHERE IdOrden = @ID),NOW(),(SELECT Cierre FROM Sesion WHERE IdSesion = (SELECT IdSesion FROM Orden WHERE IdOrden = @ID)));";
            db.Execute(insEntrada, new { ID = id });
            var insQR = "INSERT INTO CodigoQR (IdEntrada, Codigo) VALUES (@ID,(SELECT CONCAT_WS('-', Nombre, IdOrden, Emision, TipoEntrada, DNI) FROM Entrada WHERE IdEntrada = @ID));";
            db.Execute(insQR, new { ID = id });
        }
        public void CancelarOrden(int id)
        {
            var sql = "UPDATE Orden SET Cancelado = true WHERE IdOrden = @ID";
            db.Execute(sql, new { ID = id });
        }
        #endregion


        #region Entradas(Tickets)
        public IEnumerable<Entrada> GetEntradas()
        {
            var sql = "SELECT * FROM Entrada";
            return db.Query<Entrada>(sql);
        }
        public Entrada? GetEntradaById(int id)
        {
            var sql = "SELECT * FROM Entrada WHERE IdEntrada = @ID";
            return db.QueryFirstOrDefault<Entrada>(sql, new { ID = id });
        }
        public void AnularEntrada(int id)
        {
            var sql = "UPDATE Entrada SET Anulada = true WHERE IdEntrada = @ID";
            db.Execute(sql, new { ID = id });
        }
        #endregion

        #region CodigoQR

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

        #endregion
        #region Auth

        public void AuthRegistrar(Usuario usuario)
        {
            var sql = "INSERT INTO Usuario( Email, Clave) VALUES (@Email, @Clave)";
            db.Execute(sql, new
            {
                usuario.Email,
                usuario.Clave
            });
        }

        public void AuthLogin()
        {

        }
        public Usuario buscarUsuario(Usuario usuario)
        {
            var sql ="SELECT * FROM Usuario WHERE Email = @email AND Clave = @clave";
            var usuarioEncontrado = db.QueryFirstOrDefault(sql, new
            {
                usuario.Email,
                usuario.Clave
            });
            return usuarioEncontrado!;
        }
        #endregion
    }
}