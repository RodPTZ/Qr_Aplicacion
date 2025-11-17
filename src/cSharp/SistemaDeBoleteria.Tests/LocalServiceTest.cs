using Moq;
using Xunit;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Services;

public class LocalServiceTest
{
    [Fact]
    public void CuandoGetAll_DebeRetornarListaDeLocales()
    {
        // Arrange
        var repoMock = new Mock<ILocalRepository>();

        var esperado = new List<Local>
        {
            new Local(1, "Quilmes") { Nombre = "Local 1" },
            new Local(2, "Avellaneda") { Nombre = "Local 2" }
        };

        repoMock
            .Setup(r => r.SelectAll())
            .Returns(esperado);

        var service = new LocalService(repoMock.Object);

        // Act
        var resultado = service.GetAll();

        // Assert
        Assert.Equal(esperado.Count, resultado.Count());
        Assert.Equal(esperado[0].Nombre, resultado.First().Nombre);
        Assert.Equal(esperado[1].Ubicacion, resultado.Last().Ubicacion);

        repoMock.Verify(r => r.SelectAll(), Times.Once);
    }

    [Fact]
    public void CuandoGetYExiste_DebeRetornarLocal()
    {
        // Arrange
        var repoMock = new Mock<ILocalRepository>();

        var idLocal = 5;
        var local = new Local(idLocal, "Lanus") { Nombre = "Localito" };

        repoMock
            .Setup(r => r.Select(idLocal))
            .Returns(local);

        var service = new LocalService(repoMock.Object);

        // Act
        var resultado = service.Get(idLocal);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(local.IdLocal, resultado.IdLocal);
        Assert.Equal(local.Nombre, resultado.Nombre);
        Assert.Equal(local.Ubicacion, resultado.Ubicacion);

        repoMock.Verify(r => r.Select(idLocal), Times.Once);
    }

    [Fact]
    public void CuandoGetYNoExiste_DebeRetornarNull()
    {
        // Arrange
        var repoMock = new Mock<ILocalRepository>();

        var idLocal = 99;

        repoMock
            .Setup(r => r.Select(idLocal))
            .Returns((Local)null);

        var service = new LocalService(repoMock.Object);

        // Act
        var resultado = service.Get(idLocal);

        // Assert
        Assert.Null(resultado);
        repoMock.Verify(r => r.Select(idLocal), Times.Once);
    }

    [Fact]
    public void CuandoPost_DebeCrearYRetornarLocal()
    {
        // Arrange
        var repoMock = new Mock<ILocalRepository>();

        var dto = new CrearActualizarLocalDTO
        {
            Nombre = "Nuevo Local",
            Ubicacion = "Berazategui"
        };

        var localInsertado = new Local(10, dto.Ubicacion) { Nombre = dto.Nombre };

        repoMock
            .Setup(r => r.Insert(It.IsAny<Local>()))
            .Returns(localInsertado);

        var service = new LocalService(repoMock.Object);

        // Act
        var resultado = service.Post(dto);

        // Assert
        Assert.Equal(localInsertado.IdLocal, resultado.IdLocal);
        Assert.Equal(localInsertado.Nombre, resultado.Nombre);
        Assert.Equal(localInsertado.Ubicacion, resultado.Ubicacion);

        repoMock.Verify(r => r.Insert(It.IsAny<Local>()), Times.Once);
    }

    [Fact]
    public void CuandoPutYExiste_DebeActualizarYRetornarLocal()
    {
        // Arrange
        var repoMock = new Mock<ILocalRepository>();

        var id = 3;
        var dto = new CrearActualizarLocalDTO
        {
            Nombre = "Actualizado",
            Ubicacion = "Centro"
        };

        var actualizado = new Local(id, dto.Ubicacion) { Nombre = dto.Nombre };

        repoMock
            .Setup(r => r.Exists(id))
            .Returns(true);

        repoMock
            .Setup(r => r.Update(It.IsAny<Local>(), id))
            .Returns(true);

        repoMock
            .Setup(r => r.Select(id))
            .Returns(actualizado);

        var service = new LocalService(repoMock.Object);

        // Act
        var resultado = service.Put(dto, id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(actualizado.IdLocal, resultado.IdLocal);
        Assert.Equal(actualizado.Nombre, resultado.Nombre);
        Assert.Equal(actualizado.Ubicacion, resultado.Ubicacion);

        repoMock.Verify(r => r.Exists(id), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Local>(), id), Times.Once);
        repoMock.Verify(r => r.Select(id), Times.Once);
    }

    [Fact]
    public void CuandoDeleteYExisteSinEventosNiFunciones_DebeEliminarLocal()
    {
        // Arrange
        var repoMock = new Mock<ILocalRepository>();

        var id = 4;

        repoMock.Setup(r => r.Exists(id)).Returns(true);
        repoMock.Setup(r => r.HasEventos(id)).Returns(false);
        repoMock.Setup(r => r.HasFunciones(id)).Returns(false);
        repoMock.Setup(r => r.Delete(id)).Returns(true);

        var service = new LocalService(repoMock.Object);

        // Act
        service.Delete(id);

        // Assert
        repoMock.Verify(r => r.Exists(id), Times.Once);
        repoMock.Verify(r => r.HasEventos(id), Times.Once);
        repoMock.Verify(r => r.HasFunciones(id), Times.Once);
        repoMock.Verify(r => r.Delete(id), Times.Once);
    }
}