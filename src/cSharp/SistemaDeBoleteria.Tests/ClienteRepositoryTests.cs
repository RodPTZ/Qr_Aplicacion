using Moq;
using Xunit;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Services;


public class ClienteRepositoryTests
{
    [Fact]
    public void CuandoGetAll_DebeRetornarListaDeClientes()
    {
        // Arrange
        var repoMock = new Mock<IClienteRepository>();
        var repoLoginMock = new Mock<ILoginRepository>();
        var esperado = new List<Cliente>
        {
            new Cliente(1, "Juan", "Perez", "Avellaneda", 12345678, "1111", 30),
            new Cliente(2, "Ana", "Lopez", "Lanus", 87654321, "2222", 25)
        };

        repoMock
            .Setup(r => r.SelectAll())
            .Returns(esperado);

        var service = new ClienteService(repoMock.Object, repoLoginMock.Object);

        // Act
        var resultado = service.GetAll();

        // Assert
        Assert.Equal(esperado.Count, resultado.Count());
        Assert.Equal(esperado[0].Nombre, resultado.First().Nombre);
        Assert.Equal(esperado[1].Apellido, resultado.Last().Apellido);

        repoMock.Verify(r => r.SelectAll(), Times.Once);
    }

    [Fact]
    public void CuandoPutYExiste_DebeActualizarYRetornarClienteActualizado()
    {
        // Arrange
        var repoMock = new Mock<IClienteRepository>();
        var repoLoginMock = new Mock<ILoginRepository>();

        var idCliente = 5;

        var dto = new ActualizarClienteDTO
        {
            Nombre = "Nuevo",
            Apellido = "Nombre",
            Localidad = "Quilmes",
            Telefono = "5555",
        };

        var actualizado = new Cliente(
            IdCliente: idCliente,
            nombre: dto.Nombre,
            apellido: dto.Apellido,
            localidad: dto.Localidad,
            dni: 929292,
            telefono: dto.Telefono,
            edad: 100
        );

        repoMock
            .Setup(r => r.Exists(idCliente))
            .Returns(true);

        repoMock
            .Setup(r => r.Update(It.IsAny<Cliente>(), idCliente))
            .Returns(true);

        repoMock
            .Setup(r => r.Select(idCliente))
            .Returns(actualizado);

        var service = new ClienteService(repoMock.Object, repoLoginMock.Object);

        // Act
        var resultado = service.Put(dto, idCliente);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(actualizado.IdCliente, resultado.IdCliente);
        Assert.Equal(actualizado.Nombre, resultado.Nombre);
        Assert.Equal(actualizado.Apellido, resultado.Apellido);
        Assert.Equal(actualizado.Localidad, resultado.Localidad);
        Assert.Equal(actualizado.DNI, resultado.DNI);
        Assert.Equal(actualizado.Telefono, resultado.Telefono);
        Assert.Equal(actualizado.Edad, resultado.Edad);

        repoMock.Verify(r => r.Exists(idCliente), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Cliente>(), idCliente), Times.Once);
        repoMock.Verify(r => r.Select(idCliente), Times.Once);
    }
}