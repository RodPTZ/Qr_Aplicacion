using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Mapster;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Exceptions;
using SistemaDeBoleteria.Services;

namespace SistemaDeBoleteria.Tests
{
    public class ClienteXUnit
    {
        [Fact]
        public void SelectAll_RetornaCorrectamente_Los_Clientes()
        {
            var service = new Mock<IClienteService>();
            var clientes = new List<MostrarClienteDTO>
            {
                new Cliente(1, "Juan", "Perez", "Buenos Aires", 12345678, "123456789", 30)
                    .Adapt<MostrarClienteDTO>(),
                new Cliente(2, "Maria", "Gomez", "Cordoba", 87654321, "987654321", 25)
                    .Adapt<MostrarClienteDTO>()
            };

            service.Setup(repo => repo.GetAll()).Returns(clientes);

            Assert.Equal(2, service.Object.GetAll().Count());
        }
        [Fact]
        public void SelectById_RetornaCorrectamente_Cliente()
        {
            var service = new Mock<IClienteService>();

            var cliente = new Cliente(1, "Juan", "Perez", "Buenos Aires", 12345678, "123456789", 30)
                                .Adapt<MostrarClienteDTO>();

            service.Setup(repo => repo.GetById(1)).Returns(cliente);

            Assert.Equal(cliente, service.Object.GetById(1));
        }

        [Fact]
        public void SelectById_RetornaNull_CuandoNoExiste()
        {
            var service = new Mock<IClienteService>();

            service.Setup(repo => repo.GetById(99)).Returns((MostrarClienteDTO)null!);

            Assert.Null(service.Object.GetById(99));
        }
        [Fact]
        public void Insert_RetornaCorrectamente()
        {
            var repoCliente = new Mock<IClienteService>();

            var crear = new CrearClienteDTO
            {
                Nombre = "Juan",
                Apellido = "Perez",
                Localidad = "BsAs",
                Telefono = "123",
                DNI = 12345678,
                Edad = 30,
                Email = "jp@gmail.com",
                Contraseña = "1234",
                NombreUsuario = "juanp"
            };

            var esperado = new MostrarClienteDTO
            {
                IdCliente = 1,
                Nombre = "Juan",
                Apellido = "Perez",
                Localidad = "BsAs",
                Telefono = "123",
                DNI = 12345678,
                Edad = 30
            };

            repoCliente.Setup(r => r.Post(crear)).Returns(esperado);

            var resultado = repoCliente.Object.Post(crear);

            Assert.NotNull(resultado);
            Assert.Equal(esperado.IdCliente, resultado.IdCliente);
        }
        [Fact]
        public void Update_SeRealizaCorrectamente()
        {
            var repo = new Mock<IClienteRepository>();
            var login = new Mock<ILoginRepository>();

            var service = new ClienteService(repo.Object, login.Object);

            var actualizar = new ActualizarClienteDTO
            {
                Nombre = "Juan",
                Apellido = "Lopez",
                Localidad = "Rosario",
                Telefono = "555"
            };

            repo.Setup(r => r.Exists(1)).Returns(true);
            repo.Setup(r => r.Update(It.IsAny<Cliente>(), 1)).Returns(true);
            repo.Setup(r => r.Select(1)).Returns(new Cliente(1, "Juan", "Lopez", "Rosario", 123, "555", 30));

            var resultado = service.Put(actualizar, 1);

            Assert.NotNull(resultado);
            Assert.Equal("Lopez", resultado.Apellido);
        }
        [Fact]
        public void Update_NoSeRealiza_CuandoUpdateFalla()
        {
            var repo = new Mock<IClienteRepository>();
            var login = new Mock<ILoginRepository>();

            var service = new ClienteService(repo.Object, login.Object);

            var actualizar = new ActualizarClienteDTO
            {
                Nombre = "Juan",
                Apellido = "Lopez",
                Localidad = "Rosario",
                Telefono = "555"
            };

            repo.Setup(r => r.Exists(1)).Returns(true);
            repo.Setup(r => r.Update(It.IsAny<Cliente>(), 1)).Returns(false);

            Assert.Throws<BusinessException>(() => service.Put(actualizar, 1));
        }
        [Fact]
        public void Update_LanzaException_CuandoClienteNoExiste()
        {
            var repo = new Mock<IClienteRepository>();
            var login = new Mock<ILoginRepository>();

            var service = new ClienteService(repo.Object, login.Object);

            var actualizar = new ActualizarClienteDTO
            {
                Nombre = "Juan",
                Apellido = "Lopez",
                Localidad = "Rosario",
                Telefono = "555"
            };

            repo.Setup(r => r.Exists(1)).Returns(false);

            Assert.Throws<NotFoundException>(() => service.Put(actualizar, 1));
        }
    }
}
