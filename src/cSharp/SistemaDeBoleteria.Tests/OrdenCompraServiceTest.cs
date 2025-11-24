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
        var clienteRepository = new Mock<IClienteRepository>();
        var entradaRepository = new Mock<IEntradaRepository>();
        var codigoQRRepository = new Mock<ICodigoQRRepository>();

        var dto = new CrearOrdenDTO
        {
            IdTarifa = 10,
            IdCliente = 30,
            MedioDePago = ETipoDePago.Efectivo
        };

        var ordenInsertada = new Orden
        {
            IdOrden = 999,
            IdTarifa = dto.IdTarifa,
            IdCliente = dto.IdCliente,
            Estado = ETipoEstadoOrden.Creado,
            MedioDePago = dto.MedioDePago,
            Emision = DateTime.Now,
            Cierre = DateTime.Now.AddMinutes(15)
        };

        tarifaRepository.Setup(r => r.Exists(dto.IdTarifa)).Returns(true);
        tarifaRepository.Setup(r => r.ReducirStock(dto.IdTarifa)).Returns(true);
        clienteRepository.Setup(r => r.Exists(dto.IdCliente)).Returns(true);
        ordenRepository.Setup(r => r.Insert(It.IsAny<Orden>())).Returns(ordenInsertada);
        ordenRepository.Setup(r => r.Select(ordenInsertada.IdOrden)).Returns(ordenInsertada);

        var service = new OrdenService(
            ordenRepository.Object,
            tarifaRepository.Object,
            clienteRepository.Object,
            entradaRepository.Object,
            codigoQRRepository.Object
        );

        // Act
        var resultado = service.Post(dto);

        // Assert
        Assert.Equal(ordenInsertada.IdOrden, resultado.IdOrden);
        Assert.Equal(ordenInsertada.IdTarifa, resultado.IdTarifa);
        Assert.Equal(ordenInsertada.IdCliente, resultado.IdCliente);

        tarifaRepository.Verify(r => r.Exists(dto.IdTarifa), Times.Once);
        tarifaRepository.Verify(r => r.ReducirStock( dto.IdTarifa), Times.Once);
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
        var entradaRepository = new Mock<IEntradaRepository>();
        var codigoQRRepository = new Mock<ICodigoQRRepository>();

        var dto = new CrearOrdenDTO
        {
            IdTarifa = 1,
            IdCliente = 3,
            MedioDePago = ETipoDePago.Efectivo
        };

        tarifaRepository.Setup(r => r.Exists(dto.IdTarifa)).Returns(false);

        var service = new OrdenService(
            ordenRepository.Object,
            tarifaRepository.Object,
            clienteRepository.Object,
            entradaRepository.Object,
            codigoQRRepository.Object
        );

        // Act + Assert
        Assert.Throws<NotFoundException>(() => service.Post(dto));
        tarifaRepository.Verify(r => r.Exists(dto.IdTarifa), Times.Once);
    }

    // [Fact]
    // public void CuandoPost_DebeFallarSiFuncionNoExiste()
    // {
    //     var ordenRepository = new Mock<IOrdenRepository>();
    //     var tarifaRepository = new Mock<ITarifaRepository>();
    //     var funcionRepository = new Mock<IFuncionRepository>();
    //     var clienteRepository = new Mock<IClienteRepository>(); 
    //     var entradaRepository = new Mock<IEntradaRepository>();
    //     var codigoQRRepository = new Mock<ICodigoQRRepository>();

    //     var dto = new CrearOrdenDTO
    //     {
    //         IdTarifa = 10,
    //         IdCliente = 30,
    //         MedioDePago = ETipoDePago.Efectivo
    //     };

    //     tarifaRepository.Setup(r => r.Exists(dto.IdTarifa)).Returns(true);
    //     funcionRepository.Setup(r => r.Exists(dto.IdFuncion)).Returns(false);

    //     var service = new OrdenService(
    //         ordenRepository.Object,
    //         tarifaRepository.Object,
    //         funcionRepository.Object,
    //         clienteRepository.Object,
    //         entradaRepository.Object,
    //         codigoQRRepository.Object
    //     );

    //     Assert.Throws<NotFoundException>(() => service.Post(dto));
    //     funcionRepository.Verify(r => r.Exists(dto.IdFuncion), Times.Once);
    // }

    [Fact]
    public void CuandoPost_DebeFallarSiClienteNoExiste()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var tarifaRepository = new Mock<ITarifaRepository>();
        var funcionRepository = new Mock<IFuncionRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        var entradaRepository = new Mock<IEntradaRepository>();
        var codigoQRRepository = new Mock<ICodigoQRRepository>();

        var dto = new CrearOrdenDTO
        {
            IdTarifa = 10,
            IdCliente = 30,
            MedioDePago = ETipoDePago.Efectivo
        };

        tarifaRepository.Setup(r => r.Exists(dto.IdTarifa)).Returns(true);
        clienteRepository.Setup(r => r.Exists(dto.IdCliente)).Returns(false);

        var service = new OrdenService(
            ordenRepository.Object,
            tarifaRepository.Object,
            clienteRepository.Object,
            entradaRepository.Object,
            codigoQRRepository.Object
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
        var entradaRepository = new Mock<IEntradaRepository>();
        var codigoQRRepository = new Mock<ICodigoQRRepository>();

        var idOrden = 50;
        var estado = ETipoEstadoOrden.Creado;
        var cierre = DateTime.Now.AddMinutes(10);

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((ETipoEntrada.General, estado, cierre, false, 10, ETipoEstadoTarifa.Activa, ETipoEstadoEvento.Publicado));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);
        ordenRepository.Setup(r => r.UpdAbonado(idOrden)).Returns(true); 
        entradaRepository.Setup(r => r.Insert(idOrden, ETipoEntrada.General)).Returns(1);
        codigoQRRepository.Setup(r => r.Insert(1)).Returns(true);
        var service = new OrdenService(
            ordenRepository.Object,
            tarifaRepository.Object,
            clienteRepository.Object,
            entradaRepository.Object,
            codigoQRRepository.Object
        );

        var resultado = service.PagarOrden(idOrden);

        Assert.True(resultado);
        ordenRepository.Verify(r => r.UpdAbonado(idOrden), Times.Once);
    }

    [Fact]
    public void CuandoPagarOrden_DebeFallarSiYaCancelada()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null, null, null);

        var idOrden = 5;
        var estado = ETipoEstadoOrden.Cancelado;
        var cierre = DateTime.Now.AddMinutes(5);

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((ETipoEntrada.General, estado, cierre, false, 10, ETipoEstadoTarifa.Activa, ETipoEstadoEvento.Creado));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);

        Assert.Throws<BusinessException>(() => service.PagarOrden(idOrden));
    }

    [Fact]
    public void CuandoPagarOrden_DebeFallarSiYaPagada()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null,null,null);

        var idOrden = 5;
        var estado = ETipoEstadoOrden.Abonado;
        var cierre = DateTime.Now.AddMinutes(10);

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((ETipoEntrada.General,estado, cierre, false, 10, ETipoEstadoTarifa.Activa, ETipoEstadoEvento.Creado));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);

        Assert.Throws<BusinessException>(() => service.PagarOrden(idOrden));
    }

    [Fact]
    public void CuandoPagarOrden_DebeFallarSiExpirada()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var entradaRepository = new Mock<IEntradaRepository>();
        var codigoQRRepository = new Mock<ICodigoQRRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null, entradaRepository.Object, codigoQRRepository.Object);

        var idOrden = 5;
        var estado = ETipoEstadoOrden.Creado;
        var cierre = DateTime.Now.AddMinutes(-1);

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((ETipoEntrada.General, estado, cierre, false, 10, ETipoEstadoTarifa.Activa, ETipoEstadoEvento.Publicado));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);
        ordenRepository.Setup(r => r.UpdEstadoExpirado(idOrden)).Returns(true);
        ordenRepository.Setup(r => r.UpdAbonado(idOrden)).Returns(true);
        entradaRepository.Setup(r => r.Insert(idOrden,ETipoEntrada.General)).Returns(1);
        codigoQRRepository.Setup(r => r.Insert(1)).Returns(true);
        Assert.Throws<BusinessException>(() => service.PagarOrden(idOrden));
        ordenRepository.Verify(r => r.UpdEstadoExpirado(idOrden), Times.Once);
    }

    [Fact]
    public void CuandoPagarOrden_DebeFallarSiNoExiste()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null, null, null);

        var idOrden = 80;
        ordenRepository.Setup(r => r.Data(idOrden)).Returns((ETipoEntrada.General,ETipoEstadoOrden.Creado, DateTime.Now, false, 10, ETipoEstadoTarifa.Activa, ETipoEstadoEvento.Creado));
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
        var tarifaRepository = new Mock<ITarifaRepository>();
        var service = new OrdenService(ordenRepository.Object, tarifaRepository.Object, null, null, null);

        var idOrden = 10;
        var estado = ETipoEstadoOrden.Creado;
        var estadoEntrada = ETipoEntrada.General;

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((estadoEntrada,estado, DateTime.Now, false, 10, ETipoEstadoTarifa.Activa, ETipoEstadoEvento.Creado));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);
        tarifaRepository.Setup(r => r.DevolverStock(idOrden)).Returns(true);
        ordenRepository.Setup(r => r.UpdCancelado(idOrden)).Returns(true);

        var resultado = service.CancelarOrden(idOrden);

        Assert.True(resultado);
        ordenRepository.Verify(r => r.UpdCancelado(idOrden), Times.Once);
    }

    [Fact]
    public void CuandoCancelarOrden_DebeFallarSiYaCancelada()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null,null,null);

        var idOrden = 10;
        var estado = ETipoEstadoOrden.Cancelado;
        var estadoEntrada = ETipoEntrada.General;

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((estadoEntrada, estado, DateTime.Now, false, 10, ETipoEstadoTarifa.Activa, ETipoEstadoEvento.Creado));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(true);

        Assert.Throws<BusinessException>(() => service.CancelarOrden(idOrden));
    }

    [Fact]
    public void CuandoCancelarOrden_DebeFallarSiNoExiste()
    {
        var ordenRepository = new Mock<IOrdenRepository>();
        var service = new OrdenService(ordenRepository.Object, null, null, null, null);

        var idOrden = 10;

        ordenRepository.Setup(r => r.Data(idOrden)).Returns((ETipoEntrada.General,ETipoEstadoOrden.Creado, DateTime.Now, false, 10, ETipoEstadoTarifa.Activa, ETipoEstadoEvento.Creado));
        ordenRepository.Setup(r => r.Exists(idOrden)).Returns(false);

        Assert.Throws<NotFoundException>(() => service.CancelarOrden(idOrden));
    }
}
