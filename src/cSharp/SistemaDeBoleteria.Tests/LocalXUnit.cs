using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Mapster;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;

namespace SistemaDeBoleteria.Tests
{
    public class LocalXUnit
    {
        [Fact]
        public void SelectAll_RetornaCorrectamente_Los_Locales()
        {
            var localMoq = new Mock<ILocalService>();
            var locales = new List<MostrarLocalDTO>
            {
                new Local(1,"Buenos Aires").Adapt<MostrarLocalDTO>(),
                new Local(2,"Iguazú").Adapt<MostrarLocalDTO>()
            };
            
            localMoq.Setup(repo => repo.GetAll()).Returns(locales);

            Assert.Equal(2, localMoq.Object.GetAll().Count());
        }
        [Fact]
        public void Select_RetornaCorrectamente_Local()
        {
            var localMoq = new Mock<ILocalService>();
            var local = new Local(1,"Buenos Aires").Adapt<MostrarLocalDTO>();
            
            localMoq.Setup(repo => repo.Get(local.IdLocal)).Returns(local);

            Assert.Equal(local, localMoq.Object.Get(local.IdLocal));
        }
        [Fact]
        public void Insert_RetornaCorrectamente()
        {
            // Arrange
            var localMoq = new Mock<ILocalService>();

            var local = new Local(1, "Buenos Aires");

            var dtoEntrada = local.Adapt<CrearActualizarLocalDTO>();
            var dtoSalida = local.Adapt<MostrarLocalDTO>();

            localMoq
                .Setup(ser => ser.Post(dtoEntrada))
                .Returns(dtoSalida);

            // Act
            var resultado = localMoq.Object.Post(dtoEntrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(dtoSalida.IdLocal, resultado.IdLocal);
            Assert.Equal(dtoSalida.Nombre, resultado.Nombre);
        }
        [Fact]
        public void Update_SeRealizaCorrectamente()
        {
            var localMoq = new Mock<ILocalRepository>();

            var local = new Local(1,"Buenos Aires");

            localMoq.Setup(repo => repo.Update(local, 1)).Returns(true);

            var resultado = localMoq.Object.Update(local, 1);

            Assert.True(resultado);
        }
        [Fact]
        public void Update_NoSeRealizaCorrectamente()
        {
            var localMoq = new Mock<ILocalRepository>();

            var local = new Local(1,"Buenos Aires");

            localMoq.Setup(repo => repo.Update(local, 1)).Returns(false);

            var resultado = localMoq.Object.Update(local, 1);

            Assert.False(resultado);
        }
        [Fact]
        public void Select_RetornaNull_CuandoNoExisteLocal()
        {
            var localMoq = new Mock<ILocalService>();

            localMoq.Setup(ser => ser.Get(99)).Returns((MostrarLocalDTO)null!);

            var resultado = localMoq.Object.Get(99);

            Assert.Null(resultado);
        }
        
    }
}