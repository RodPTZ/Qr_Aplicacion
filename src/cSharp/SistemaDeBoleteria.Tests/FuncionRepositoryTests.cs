using Xunit;
using Moq;
using SistemaDeBoleteria.Services;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Exceptions;
using System;

public class FuncionRepositoryTests
{
    [Fact]
    public void CuandoPostFuncion_DebeCrearFuncion()
    {
        // Arrange
        var funcionRepo = new Mock<IFuncionRepository>();
        var eventoRepo = new Mock<IEventoRepository>();
        var sectorRepo = new Mock<ISectorRepository>();
        var entradaRepo = new Mock<IEntradaRepository>();
        var tarifaRepo = new Mock<ITarifaRepository>();

        var dto = new CrearFuncionDTO
        {
            IdEvento = 1,
            IdSector = 2,
            Fecha = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            AperturaTime = new TimeOnly(10, 0),
            CierreTime = new TimeOnly(12, 0)
        };

        var funcionInsertada = new Funcion
        {
            IdFuncion = 5,
            IdEvento = dto.IdEvento,
            IdSector = dto.IdSector,
            Fecha = dto.Fecha,
            AperturaTime = dto.AperturaTime,
            CierreTime = dto.CierreTime
        };

        eventoRepo.Setup(r => r.Exists(dto.IdEvento)).Returns(true);
        sectorRepo.Setup(r => r.Exists(dto.IdSector)).Returns(true);
        funcionRepo.Setup(r => r.Insert(It.IsAny<Funcion>())).Returns(funcionInsertada);

        var service = new FuncionService(funcionRepo.Object, eventoRepo.Object, sectorRepo.Object, entradaRepo.Object, tarifaRepo.Object);

        // Act
        var result = service.Post(dto);

        // Assert
        Assert.Equal(funcionInsertada.IdFuncion, result.IdFuncion);
        Assert.Equal(dto.IdEvento, result.IdEvento);
        Assert.Equal(dto.IdSector, result.IdSector);
        Assert.Equal(dto.Fecha, result.Fecha);
        Assert.Equal(dto.AperturaTime, result.AperturaTime);
        Assert.Equal(dto.CierreTime, result.CierreTime);

        eventoRepo.Verify(r => r.Exists(dto.IdEvento));
        sectorRepo.Verify(r => r.Exists(dto.IdSector));
        funcionRepo.Verify(r => r.Insert(It.IsAny<Funcion>()));
    }

    [Fact]
    public void CuandoPostFuncionConEventoInexistente_DebeLanzarNotFound()
    {
        // Arrange
        var funcionRepo = new Mock<IFuncionRepository>();
        var eventoRepo = new Mock<IEventoRepository>();
        var sectorRepo = new Mock<ISectorRepository>();
        var entradaRepo = new Mock<IEntradaRepository>();
        var tarifaRepo = new Mock<ITarifaRepository>();
        var dto = new CrearFuncionDTO
        {
            IdEvento = 99,
            IdSector = 1,
            Fecha = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            AperturaTime = new TimeOnly(10, 0),
            CierreTime = new TimeOnly(12, 0)
        };

        eventoRepo.Setup(r => r.Exists(dto.IdEvento)).Returns(false);

        var service = new FuncionService(funcionRepo.Object, eventoRepo.Object, sectorRepo.Object, entradaRepo.Object, tarifaRepo.Object);

        // Act & Assert
        Assert.Throws<NotFoundException>(() => service.Post(dto));

        eventoRepo.Verify(r => r.Exists(dto.IdEvento));
    }

    [Fact]
    public void CuandoPutFuncion_DebeActualizarFuncion()
    {
        // Arrange
        var funcionRepo = new Mock<IFuncionRepository>();
        var eventoRepo = new Mock<IEventoRepository>();
        var sectorRepo = new Mock<ISectorRepository>();
        var entradaRepo = new Mock<IEntradaRepository>();
        var tarifaRepo = new Mock<ITarifaRepository>();
        int idFuncion = 10;

        var dto = new ActualizarFuncionDTO
        {
            IdSector = 3,
            Fecha = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            AperturaTime = new TimeOnly(14, 0),
            CierreTime = new TimeOnly(16, 0)
        };

        var funcion = new Funcion
        {
            IdFuncion = idFuncion,
            IdSector = dto.IdSector,
            Fecha = dto.Fecha,
            AperturaTime = dto.AperturaTime,
            CierreTime = dto.CierreTime
        };

        funcionRepo.Setup(r => r.Exists(idFuncion)).Returns(true);
        sectorRepo.Setup(r => r.Exists(dto.IdSector)).Returns(true);
        funcionRepo.Setup(r => r.Update(It.IsAny<Funcion>(), idFuncion)).Returns(true);
        funcionRepo.Setup(r => r.Select(idFuncion)).Returns(funcion);

        var service = new FuncionService(funcionRepo.Object, eventoRepo.Object, sectorRepo.Object, entradaRepo.Object, tarifaRepo.Object);

        // Act
        var result = service.Put(dto, idFuncion);

        // Assert
        Assert.Equal(dto.IdSector, result.IdSector);
        Assert.Equal(dto.Fecha, result.Fecha);
        Assert.Equal(dto.AperturaTime, result.AperturaTime);
        Assert.Equal(dto.CierreTime, result.CierreTime);

        funcionRepo.Verify(r => r.Exists(idFuncion));
        sectorRepo.Verify(r => r.Exists(dto.IdSector));
        funcionRepo.Verify(r => r.Update(It.IsAny<Funcion>(), idFuncion));
    }

    [Fact]
    public void CuandoCancelarFuncion_DebeCancelar()
    {
        // Arrange
        var funcionRepo = new Mock<IFuncionRepository>();
        var eventoRepo = new Mock<IEventoRepository>();
        var sectorRepo = new Mock<ISectorRepository>();
        var entradaRepo = new Mock<IEntradaRepository>();
        var tarifaRepo = new Mock<ITarifaRepository>();
        int idFuncion = 7;

        funcionRepo.Setup(r => r.Exists(idFuncion)).Returns(true);
        entradaRepo.Setup(r => r.UpdAnularEntradasDeFuncionID(idFuncion)).Returns(true);
        tarifaRepo.Setup(r => r.SuspenderTarifasPorIdFuncion(idFuncion)).Returns(true);
        funcionRepo.Setup(r => r.UpdFuncionCancel(idFuncion)).Returns(true);

        var service = new FuncionService(funcionRepo.Object, eventoRepo.Object, sectorRepo.Object, entradaRepo.Object, tarifaRepo.Object);

        // Act
        service.Cancelar(idFuncion);

        // Assert
        funcionRepo.Verify(r => r.Exists(idFuncion));
        funcionRepo.Verify(r => r.UpdFuncionCancel(idFuncion));
    }
}
