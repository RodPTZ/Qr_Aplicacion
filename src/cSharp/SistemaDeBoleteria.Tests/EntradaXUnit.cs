using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Mapster;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Exceptions;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Services;

namespace SistemaDeBoleteria.Tests
{
    public class EntradaXUnit
    {
        [Fact]
        public void SelectAll_RetornaCorrectamente_Las_Entradas()
        {
            var entradaRepo = new Mock<IEntradaRepository>();

            var entradas = new List<Entrada>
            {
                new Entrada { IdEntrada = 1, IdOrden = 10, Estado = ETipoEstadoEntrada.Pendiente },
                new Entrada { IdEntrada = 2, IdOrden = 11, Estado = ETipoEstadoEntrada.Pendiente }
            };

            entradaRepo.Setup(r => r.SelectAll()).Returns(entradas);

            var service = new EntradaService(entradaRepo.Object);

            var resultado = service.GetAll();

            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public void Select_RetornaEntradaCorrecta()
        {
            var entradaRepo = new Mock<IEntradaRepository>();

            var entrada = new Entrada { IdEntrada = 1, IdOrden = 10, Estado = ETipoEstadoEntrada.Pendiente };

            entradaRepo.Setup(r => r.Select(1)).Returns(entrada);

            var service = new EntradaService(entradaRepo.Object);

            var resultado = service.GetById(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.IdEntrada);
        }

        [Fact]
        public void AnularEntrada_SeRealizaCorrectamente()
        {
            var entradaRepo = new Mock<IEntradaRepository>();

            var entrada = new Entrada { IdEntrada = 1, Estado = ETipoEstadoEntrada.Pendiente };

            entradaRepo.Setup(r => r.Select(1)).Returns(entrada);
            entradaRepo.Setup(r => r.UpdateEstado(1)).Returns(true);

            var service = new EntradaService(entradaRepo.Object);

            var resultado = service.AnularEntrada(1);

            Assert.True(resultado);
        }

        [Fact]
        public void AnularEntrada_ErrorCuandoNoExiste()
        {
            var entradaRepo = new Mock<IEntradaRepository>();

            entradaRepo.Setup(r => r.Select(99)).Returns((Entrada)null!);

            var service = new EntradaService(entradaRepo.Object);

            Assert.Throws<NotFoundException>(() => service.AnularEntrada(99));
        }

        [Fact]
        public void AnularEntrada_ErrorCuandoYaEstaAnulada()
        {
            var entradaRepo = new Mock<IEntradaRepository>();

            var entrada = new Entrada { IdEntrada = 1, Estado = ETipoEstadoEntrada.Anulado };

            entradaRepo.Setup(r => r.Select(1)).Returns(entrada);

            var service = new EntradaService(entradaRepo.Object);

            Assert.Throws<BusinessException>(() => service.AnularEntrada(1));
        }
    }
}
