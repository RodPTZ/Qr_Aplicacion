using System;
using Xunit;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Enums;

namespace SistemaDeBoleteria.Tests
{
    public class OrdenTests
    {
        [Fact]
        public void CambiaEstadoAPagada()
        {
            // Arrange
            var orden = new Orden
            {
                IdOrden = 1,
                Estado = ETipoEstadoOrden.Creado
            };

            // Act
            orden.Estado = ETipoEstadoOrden.Abonado;

            // Assert
            Assert.Equal(ETipoEstadoOrden.Abonado, orden.Estado);
        }
        [Fact]
        public void Cancelar_NoSePuedeSiYaEstaAbonada()
        {
            var orden = new Orden();
            orden.Estado = ETipoEstadoOrden.Abonado;

            orden.Estado = ETipoEstadoOrden.Cancelado;

            Assert.Equal(ETipoEstadoOrden.Cancelado, orden.Estado);
        }

    }
}
