using System;
using Xunit;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Enums;

namespace SistemaDeBoleteria.Tests
{
    public class OrdenTests
    {
        [Fact]
        public void ConstructorCompleto_DeberiaAsignarCorrectamente()
        {
            // Arrange
            int idTarifa = 10;
            int idCliente = 20;
            int idFuncion = 30;
            var estado = ETipoEstadoOrden.Creado;
            var medioPago = ETipoDePago.Debito;
            DateTime emision = DateTime.Now;
            DateTime cierre = emision.AddMinutes(15);

            // Act
            Orden orden = new Orden(
                idTarifa,
                idCliente,
                idFuncion,
                estado,
                medioPago,
                emision,
                cierre
            );

            // Assert
            Assert.Equal(idTarifa, orden.IdTarifa);
            Assert.Equal(idCliente, orden.IdCliente);
            Assert.Equal(idFuncion, orden.IdFuncion);
            Assert.Equal(estado, orden.Estado);
            Assert.Equal(medioPago, orden.MedioDePago);
            Assert.Equal(emision, orden.Emision);
            Assert.Equal(cierre, orden.Cierre);
        }

        [Fact]
        public void ConstructorVacio_DeberiaCrearObjeto()
        {
            // Act
            Orden orden = new Orden();

            // Assert
            Assert.NotNull(orden);
        }

        [Fact]
        public void Propiedades_DeberianAsignarseYLeerseCorrectamente()
        {
            // Arrange
            Orden orden = new Orden();

            orden.IdOrden = 1;
            orden.IdTarifa = 2;
            orden.IdCliente = 3;
            orden.IdFuncion = 4;
            orden.Estado = ETipoEstadoOrden.Abonado;
            orden.MedioDePago = ETipoDePago.Credito;
            orden.Emision = DateTime.Now;
            orden.Cierre = DateTime.Now.AddMinutes(10);

            // Assert
            Assert.Equal(1, orden.IdOrden);
            Assert.Equal(2, orden.IdTarifa);
            Assert.Equal(3, orden.IdCliente);
            Assert.Equal(4, orden.IdFuncion);
            Assert.Equal(ETipoEstadoOrden.Abonado, orden.Estado);
            Assert.Equal(ETipoDePago.Credito, orden.MedioDePago);
            Assert.True(orden.Cierre > orden.Emision);
        }

        [Fact]
        public void Estado_DeberiaPermitirTodosLosValoresDelEnum()
        {
            Orden o = new Orden();

            o.Estado = ETipoEstadoOrden.Creado;
            Assert.Equal(ETipoEstadoOrden.Creado, o.Estado);

            o.Estado = ETipoEstadoOrden.Abonado;
            Assert.Equal(ETipoEstadoOrden.Abonado, o.Estado);

            o.Estado = ETipoEstadoOrden.Cancelado;
            Assert.Equal(ETipoEstadoOrden.Cancelado, o.Estado);
        }

        [Fact]
        public void MedioDePago_DeberiaPermitirTodosLosValoresDelEnum()
        {
            Orden o = new Orden();

            o.MedioDePago = ETipoDePago.Efectivo;
            Assert.Equal(ETipoDePago.Efectivo, o.MedioDePago);

            o.MedioDePago = ETipoDePago.Transferencia;
            Assert.Equal(ETipoDePago.Transferencia, o.MedioDePago);

            o.MedioDePago = ETipoDePago.Debito;
            Assert.Equal(ETipoDePago.Debito, o.MedioDePago);

            o.MedioDePago = ETipoDePago.Credito;
            Assert.Equal(ETipoDePago.Credito, o.MedioDePago);
        }
    }
}
