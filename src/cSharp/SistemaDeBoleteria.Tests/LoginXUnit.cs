using System;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using SistemaDeBoleteria.Services;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Exceptions;

namespace SistemaDeBoleteria.Tests
{
    public class LoginXUnit
    {
        [Fact]
public void Login_Correcto_DevuelveRespuesta()
{
    // Arrange
    var tokenRepo = new Mock<ITokenRepository>();
    var loginRepo = new Mock<ILoginRepository>();

    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Jwt:Key", "ClaveSuperMegaUltraDuperCaudrupleEsclerosisMultipleSegura" },
            { "Jwt:Issuer", "Test" }
        })
        .Build();

    var usuarioMock = new Usuario
    {
        IdUsuario = 1,
        Email = "a@mail.com",
        Rol = Core.Enums.ERolUsuario.Cliente
    };

    loginRepo
        .Setup(x => x.SelectByEmailAndPass("a@mail.com", "123"))
        .Returns(usuarioMock);

    tokenRepo
        .Setup(x => x.InsertToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
        .Returns(true);

    var service = new LoginService(tokenRepo.Object, loginRepo.Object, config);

    var loginRequest = new LoginRequest
    {
        Email = "a@mail.com",
        Contraseña = "123"
    };

    // Act
    var result = service.Login(loginRequest);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("a@mail.com", result.Email);

    Assert.NotNull(result.AccessToken);
    Assert.True(result.AccessToken.Length > 20);
}



        [Fact]
        public void Login_Incorrecto_LanzaExcepcion()
        {
            var tokenRepo = new Mock<ITokenRepository>();
            var loginRepo = new Mock<ILoginRepository>();
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();

            loginRepo.Setup(x => x.SelectByEmailAndPass(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns((Usuario?)null);

            var service = new LoginService(tokenRepo.Object, loginRepo.Object, config);

            Assert.Throws<NotFoundException>(() => service.Login(new LoginRequest { Email = "x", Contraseña = "y" }));
        }
    }
}