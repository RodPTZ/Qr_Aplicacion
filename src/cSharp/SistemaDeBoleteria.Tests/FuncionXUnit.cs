using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Exceptions;
using Mapster;

namespace SistemaDeBoleteria.Tests
{
    public class FuncionXUnit
    {
        [Fact]
        public void SelectAll_RetornaCorrectamente_Las_Funciones()
        {
            var funcionMoq = new Mock<IFuncionService>();
            var funciones = new List<MostrarFuncionDTO>
            {
                new MostrarFuncionDTO { IdFuncion = 1, IdEvento = 1, IdSector = 1, Fecha = DateOnly.FromDateTime(DateTime.Now), AperturaTime = TimeOnly.FromDateTime(DateTime.Now), CierreTime = TimeOnly.FromDateTime(DateTime.Now), Cancelado = false },
                new MostrarFuncionDTO { IdFuncion = 2, IdEvento = 2, IdSector = 2, Fecha = DateOnly.FromDateTime(DateTime.Now), AperturaTime = TimeOnly.FromDateTime(DateTime.Now), CierreTime = TimeOnly.FromDateTime(DateTime.Now), Cancelado = false }
            };

            funcionMoq.Setup(repo => repo.GetAll()).Returns(funciones);

            Assert.Equal(2, funcionMoq.Object.GetAll().Count());
        }

        [Fact]
        public void Select_RetornaCorrectamente_Funcion()
        {
            var funcionMoq = new Mock<IFuncionService>();
            var funcion = new MostrarFuncionDTO { IdFuncion = 1, IdEvento = 1, IdSector = 1, Fecha = DateOnly.FromDateTime(DateTime.Now), AperturaTime = TimeOnly.FromDateTime(DateTime.Now), CierreTime = TimeOnly.FromDateTime(DateTime.Now), Cancelado = false };

            funcionMoq.Setup(repo => repo.Get(1)).Returns(funcion);

            Assert.Equal(funcion, funcionMoq.Object.Get(1));
        }

        [Fact]
        public void Insert_RetornaCorrectamente()
        {
            var funcionMoq = new Mock<IFuncionService>();
            var funcion = new CrearFuncionDTO
            {
                IdEvento = 1,
                IdSector = 1,
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                AperturaTime = TimeOnly.FromDateTime(DateTime.Now),
                CierreTime = TimeOnly.FromDateTime(DateTime.Now)
            };

            var funcionDto = new MostrarFuncionDTO { IdFuncion = 1, IdEvento = 1, IdSector = 1, Fecha = DateOnly.FromDateTime(DateTime.Now), AperturaTime = TimeOnly.FromDateTime(DateTime.Now), CierreTime = TimeOnly.FromDateTime(DateTime.Now), Cancelado = false };

            funcionMoq.Setup(ser => ser.Post(funcion)).Returns(funcionDto);

            var resultado = funcionMoq.Object.Post(funcion);

            Assert.NotNull(resultado);
        }

        [Fact]
        public void Update_SeRealizaCorrectamente()
        {
            var funcionMoq = new Mock<IFuncionRepository>();
            var funcion = new ActualizarFuncionDTO { IdSector = 1, Fecha = DateOnly.FromDateTime(DateTime.Now), AperturaTime = TimeOnly.FromDateTime(DateTime.Now), CierreTime = TimeOnly.FromDateTime(DateTime.Now) };

            funcionMoq.Setup(repo => repo.Update(It.IsAny<Funcion>(), 1)).Returns(true);

            var resultado = funcionMoq.Object.Update(funcion.Adapt<Funcion>(), 1);

            Assert.True(resultado);
        }

        [Fact]
        public void Update_NoSeRealizaCorrectamente()
        {
            var funcionMoq = new Mock<IFuncionRepository>();
            var funcion = new ActualizarFuncionDTO { IdSector = 1, Fecha = DateOnly.FromDateTime(DateTime.Now), AperturaTime = TimeOnly.FromDateTime(DateTime.Now), CierreTime = TimeOnly.FromDateTime(DateTime.Now) };

            funcionMoq.Setup(repo => repo.Update(It.IsAny<Funcion>(), 1)).Returns(false);

            var resultado = funcionMoq.Object.Update(funcion.Adapt<Funcion>(), 1);

            Assert.False(resultado);
        }

        [Fact]
        public void Select_RetornaNull_CuandoNoExisteFuncion()
        {
            var funcionMoq = new Mock<IFuncionService>();

            funcionMoq.Setup(ser => ser.Get(99)).Returns((MostrarFuncionDTO)null!);

            var resultado = funcionMoq.Object.Get(99);

            Assert.Null(resultado);
        }

        [Fact]
        public void Post_LanzaException_CuandoNoExisteEvento()
        {
            var funcionMoq = new Mock<IFuncionService>();
            var funcion = new CrearFuncionDTO
            {
                IdEvento = 99,  // Evento no existe
                IdSector = 1,
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                AperturaTime = TimeOnly.FromDateTime(DateTime.Now),
                CierreTime = TimeOnly.FromDateTime(DateTime.Now)
            };

            funcionMoq.Setup(ser => ser.Post(funcion)).Throws(new NotFoundException("No se encontró el evento especificado."));

            Assert.Throws<NotFoundException>(() => funcionMoq.Object.Post(funcion));
        }

        [Fact]
        public void Cancelar_LanzaException_CuandoNoExisteFuncion()
        {
            var funcionMoq = new Mock<IFuncionService>();

            funcionMoq.Setup(ser => ser.Cancelar(99)).Throws(new NotFoundException("No se encontró la función especificada"));

            Assert.Throws<NotFoundException>(() => funcionMoq.Object.Cancelar(99));
        }
    }
}
