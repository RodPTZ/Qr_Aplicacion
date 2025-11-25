using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using SistemaDeBoleteria.Services;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.Exceptions;
using Mapster;
using System.Data.Common;

public class EntradaRepositoryTests
{
    [Fact]
    public void CuandoGetAll_DebeRetornarListaDeEntradas()
    {
        // Arrange
        var repoMock = new Mock<IEntradaRepository>();
        var codigoMock = new Mock<ICodigoQRRepository>();
        var entradas = new List<Entrada>
        {
            new Entrada
            {
                IdEntrada = 1,
                IdOrden = 10,
                TipoEntrada = ETipoEntrada.General,
                Emision = DateTime.Now,
                Liquidez = DateTime.Now.AddDays(1),
                Anulado = false
            }
        };

        repoMock
            .Setup(r => r.SelectAll())
            .Returns(entradas);

        var service = new EntradaService(repoMock.Object, codigoMock.Object);

        // Act
        var result = service.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entradas.Count, result.Count());
    }

    [Fact]
    public void CuandoGetById_DebeRetornarEntrada()
    {
        // Arrange
        var repoMock = new Mock<IEntradaRepository>();
        var codigoMock = new Mock<ICodigoQRRepository>();
        var entrada = new Entrada
        {
            IdEntrada = 5,
            IdOrden = 20,
            TipoEntrada = ETipoEntrada.VIP,
            Emision = DateTime.Now,
            Liquidez = DateTime.Now.AddDays(3),
            Anulado = false
        };

        var idEntrada = entrada.IdEntrada;

        repoMock
            .Setup(r => r.Select(idEntrada))
            .Returns(entrada);

        var service = new EntradaService(repoMock.Object, codigoMock.Object);

        // Act
        var result = service.GetById(idEntrada);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entrada.IdEntrada, result.IdEntrada);
        Assert.Equal(entrada.IdOrden, result.IdOrden);
        Assert.Equal(entrada.Emision, result.Emision);
        Assert.Equal(entrada.Liquidez, result.Liquidez);
    }

    [Fact]
    public void CuandoAnularEntrada_DebeAnularEntradaCorrectamente()
    {
        // Arrange
        var repoMock = new Mock<IEntradaRepository>();
        var codigoMock = new Mock<ICodigoQRRepository>();
        var entrada = new Entrada
        {
            IdEntrada = 7,
            IdOrden = 99,
            Anulado = false,
            Emision = DateTime.Now,
            Liquidez = DateTime.Now.AddDays(5),
            TipoEntrada = ETipoEntrada.General
        };

        var idEntrada = entrada.IdEntrada;

        repoMock
            .Setup(r => r.Select(idEntrada))
            .Returns(entrada);
        repoMock
            .Setup(r => r.UpdAnular(idEntrada))
            .Returns(true);
        codigoMock
            .Setup(c => c.UpdAYaUsada(idEntrada))
            .Returns(true);
        var service = new EntradaService(repoMock.Object, codigoMock.Object);

        // Act
        var result = service.AnularEntrada(idEntrada);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.UpdAnular(idEntrada), Times.Once);
    }

    [Fact]
    public void CuandoAnularEntradaNoExiste_DebeLanzarNotFoundException()
    {
        // Arrange
        var repoMock = new Mock<IEntradaRepository>();
        var codigoMock = new Mock<ICodigoQRRepository>();
        var idEntrada = 44;

        repoMock
            .Setup(r => r.Select(idEntrada))
            .Returns((Entrada)null);

        var service = new EntradaService(repoMock.Object, codigoMock.Object);

        // Act y Assert
        Assert.Throws<NotFoundException>(() => service.AnularEntrada(idEntrada));
    }

    [Fact]
    public void CuandoEntradaYaEstaAnulada_DebeLanzarBusinessException()
    {
        // Arrange
        var repoMock = new Mock<IEntradaRepository>();
        var codigoMock = new Mock<ICodigoQRRepository>();
        var entrada = new Entrada
        {
            IdEntrada = 9,
            Anulado = true
        };

        var idEntrada = entrada.IdEntrada;

        repoMock
            .Setup(r => r.Select(idEntrada))
            .Returns(entrada);

        var service = new EntradaService(repoMock.Object, codigoMock.Object);

        // Act y Assert
        Assert.Throws<BusinessException>(() => service.AnularEntrada(idEntrada));
    }
}