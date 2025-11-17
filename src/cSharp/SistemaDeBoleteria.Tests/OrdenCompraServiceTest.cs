using Moq;
using Xunit;
using System;
using SistemaDeBoleteria.Services;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Exceptions;
using MySqlConnector;

public class OrdenServiceTests
{
    /* ----------------------------------------------------------
       POST
       ---------------------------------------------------------- */

    [Fact]
    public void CuandoPost_DebeCrearOrdenCorrectamente()
    {
        // Arrange
        var ordenRepository = new Mock<IOrdenRepository>();
        var tarifaRepository = new Mock<ITarifaRepository>();
        var funcionRepository = new Mock<IFuncionRepository>();
        var clienteRepository = new Mock<IClienteRepository>();

        var dto = new CrearOrdenDTO
        {
            IdTarifa = 10,
            IdFuncion = 20,
            IdCliente = 30,
            MedioDePago = ETipoDePago.Efectivo
        };

        var ordenInsertada = new Orden
        {
            IdOrden = 999,
            IdTarifa = dto.IdTarifa,
            IdFuncion = dto.IdFuncion,
            IdCliente = dto.IdCliente,
            Estado = ETipoEstadoOrden.Creado,
            MedioDePago = dto.MedioDePago,
            Emision = DateTime.Now,
            Cierre = DateTime.Now.AddMinutes(15)
        };

        tarifaRepository.Setup(r => r.Exists(dto.IdTarifa)).Returns(true);
        funcionRepository.Setup(r => r.Exists(dto.IdFuncion)).Returns(true);
        clienteRepository.Setup(r => r.Exists(dto.IdCliente)).Returns(true);
        ordenRepository.Setup(r => r.Insert(It.IsAny<Orden>())).Returns(ordenInsertada);

        var service = new OrdenService(
            ordenRepository.Object,
            tarifaRepository.Object,
            funcionRepository.Object,
            clienteRepository.Object
        );

        // Act
        var resultado = service.Post(dto);

        // Assert
        Assert.Equal(ordenInsertada.IdOrden, resultado.IdOrden);
        Assert.Equal(ordenInsertada.IdTarifa, resultado.IdTarifa);
        Assert.Equal(ordenInsertada.IdCliente, resultado.IdCliente);
        Assert.Equal(ordenInsertada.IdFuncion, resultado.IdFuncion);

        tarifaRepository.Verify(r => r.Exists(dto.IdTarifa), Times.Once);
        funcionRepository.Verify(r => r.Exists(dto.IdFuncion), Times.Once);
        clienteRepository.Verify(r => r.Exists(dto.IdCliente), Times.Once);
        ordenRepository.Verify(r => r.Insert(It.IsAny<Orden>()), Times.Once);
    }

    [Fact]
    public void CuandoPost_DebeFallarSiTarifaNoExiste()
    {
        // Arrange
        var ordenRepository = new Mock<IOrdenRepository>();
        var tarifaRepository = new Mock<ITarifaRepository>();
        var funcionRepository = new Mock<IFuncionRepository>();
        var clienteRepository = new Mock<IClienteRepository>();

        var dto = new CrearOrdenDTO
        {
            IdTarifa = 1,
            IdFuncion = 2,
            IdCliente = 3,
            MedioDePago = ETipoDePago.Efectivo
        };

        tarifaRepository.Setup(r => r.Exists(dto.IdTarifa)).Returns(false);

        var service = new OrdenService(
            ordenRepository.Object,
            tarifaRepository.Object,
            funcionRepository.Object,
            clienteRepository.Object
        );

        // Act + Assert
        Assert.Throws<NotFoundException>(() => service.Post(dto));
        tarifaRepository.Verify(r => r.Exists(dto.IdTarifa), Times.Once);
    }

    [Fact]
    public void CuandoPost_DebeFallarSiFuncionNoExiste()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var tarifaRepository = new Mock<ITarifaRepository>();
        var funcionRepository = new Mock<IFuncionRepository>();
        var clienteRepository = new Mock<IClienteRepository>();

        var dto = new CrearOrdenDTO
        {
            IdTarifa = 10,
            IdFuncion = 20,
            IdCliente = 30,
            MedioDePago = ETipoDePago.Efectivo
        };

        tarifaRepository.Setup(r => r.Exists(dto.IdTarifa)).Returns(true);
        funcionRepository.Setup(r => r.Exists(dto.IdFuncion)).Returns(false);

        var service = new OrdenService(
            ordenRepository.Object,
            tarifaRepository.Object,
            funcionRepository.Object,
            clienteRepository.Object
        );

        Assert.Throws<NotFoundException>(() => service.Post(dto));
        funcionRepository.Verify(r => r.Exists(dto.IdFuncion), Times.Once);
    }

    [Fact]
    public void CuandoPost_DebeFallarSiClienteNoExiste()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var tarifaRepository = new Mock<ITarifaRepository>();
        var funcionRepository = new Mock<IFuncionRepository>();
        var clienteRepository = new Mock<IClienteRepository>();

        var dto = new CrearOrdenDTO
        {
            IdTarifa = 10,
            IdFuncion = 20,
            IdCliente = 30,
            MedioDePago = ETipoDePago.Efectivo
        };

        tarifaRepository.Setup(r => r.Exists(dto.IdTarifa)).Returns(true);
        funcionRepository.Setup(r => r.Exists(dto.IdFuncion)).Returns(true);
        clienteRepository.Setup(r => r.Exists(dto.IdCliente)).Returns(false);

        var service = new OrdenService(
            ordenRepository.Object,
            tarifaRepository.Object,
            funcionRepository.Object,
            clienteRepository.Object
        );

        Assert.Throws<NotFoundException>(() => service.Post(dto));
        clienteRepository.Verify(r => r.Exists(dto.IdCliente), Times.Once);
    }

    /* ----------------------------------------------------------
       PAGAR ORDEN
       ---------------------------------------------------------- */

    [Fact]
    public void CuandoPagarOrden_DebePagarCorrectamente()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var tarifaRepository = new Mock<ITarifaRepository>();
        var funcionRepository = new Mock<IFuncionRepository>();
        var clienteRepository = new Mock<IClienteRepository>();

        var idOrden = 50;
        var estado = ETipoEstadoOrden.Creado;
        var cierre = DateTime.Now.AddMinutes(10);

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((estado, cierre));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);
        ordenRepository.Setup(r => r.UpdEstadoPagado(idOrden)).Returns(true);

        var service = new OrdenService(
            ordenRepository.Object,
            tarifaRepository.Object,
            funcionRepository.Object,
            clienteRepository.Object
        );

        var resultado = service.PagarOrden(idOrden);

        Assert.True(resultado);
        ordenRepository.Verify(r => r.UpdEstadoPagado(idOrden), Times.Once);
    }

    [Fact]
    public void CuandoPagarOrden_DebeFallarSiYaCancelada()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null, null);

        var idOrden = 5;
        var estado = ETipoEstadoOrden.Cancelado;
        var cierre = DateTime.Now.AddMinutes(5);

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((estado, cierre));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);

        Assert.Throws<BusinessException>(() => service.PagarOrden(idOrden));
    }

    [Fact]
    public void CuandoPagarOrden_DebeFallarSiYaPagada()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null, null);

        var idOrden = 5;
        var estado = ETipoEstadoOrden.Abonado;
        var cierre = DateTime.Now.AddMinutes(10);

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((estado, cierre));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);

        Assert.Throws<BusinessException>(() => service.PagarOrden(idOrden));
    }

    [Fact]
    public void CuandoPagarOrden_DebeFallarSiExpirada()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null, null);

        var idOrden = 5;
        var estado = ETipoEstadoOrden.Creado;
        var cierre = DateTime.Now.AddMinutes(-1);

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((estado, cierre));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);
        ordenRepository.Setup(r => r.UpdEstadoExpirado(idOrden)).Returns(true);

        Assert.Throws<BusinessException>(() => service.PagarOrden(idOrden));
        ordenRepository.Verify(r => r.UpdEstadoExpirado(idOrden), Times.Once);
    }

    [Fact]
    public void CuandoPagarOrden_DebeFallarSiNoExiste()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null, null);

        var idOrden = 80;
        ordenRepository.Setup(r => r.Data(idOrden)).Returns((ETipoEstadoOrden.Creado, DateTime.Now));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(false);

        Assert.Throws<NotFoundException>(() => service.PagarOrden(idOrden));
    }

    /* ----------------------------------------------------------
       CANCELAR ORDEN
       ---------------------------------------------------------- */

    [Fact]
    public void CuandoCancelarOrden_DebeCancelarCorrectamente()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null, null);

        var idOrden = 10;
        var estado = ETipoEstadoOrden.Creado;

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((estado, DateTime.Now));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);
        ordenRepository.Setup(r => r.UpdEstadoCancelado(idOrden)).Returns(true);

        var resultado = service.CancelarOrden(idOrden);

        Assert.True(resultado);
        ordenRepository.Verify(r => r.UpdEstadoCancelado(idOrden), Times.Once);
    }

    [Fact]
    public void CuandoCancelarOrden_DebeFallarSiYaCancelada()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null, null);

        var idOrden = 10;
        var estado = ETipoEstadoOrden.Cancelado;

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((estado, DateTime.Now));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);

        Assert.Throws<BusinessException>(() => service.CancelarOrden(idOrden));
    }

    [Fact]
    public void CuandoCancelarOrden_DebeFallarSiNoExiste()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null, null);

        var idOrden = 10;

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((ETipoEstadoOrden.Creado, DateTime.Now));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(false);

        Assert.Throws<NotFoundException>(() => service.CancelarOrden(idOrden));
    }
}
