using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Services;
using SistemaDeBoleteria.Core.Exceptions;
using Mapster;

namespace SistemaDeBoleteria.Tests
{
    public class TarifaXUnit
    {
        [Fact]
        public void GetAllByFuncionId_RetornaCorrectamente_Las_Tarifas()
        {
            // Arrange
            var tarifaRepoMoq = new Mock<ITarifaRepository>();
            var funcionRepoMoq = new Mock<IFuncionRepository>();
            var tarifaService = new TarifaService(tarifaRepoMoq.Object, funcionRepoMoq.Object);

            var tarifas = new List<Tarifa>
            {
                new Tarifa { IdTarifa = 1, IdFuncion = 1, TipoEntrada = ETipoEntrada.General, Precio = 100m, Stock = 50 },
                new Tarifa { IdTarifa = 2, IdFuncion = 1, TipoEntrada = ETipoEntrada.VIP, Precio = 200m, Stock = 30 }
            };

            var tarifasDTO = tarifas.Adapt<IEnumerable<MostrarTarifaDTO>>();
            tarifaRepoMoq.Setup(repo => repo.SelectAllByFuncionId(It.IsAny<int>())).Returns(tarifas);

            // Act
            var result = tarifaService.GetAllByFuncionId(1);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.TipoEntrada == "General");
            Assert.Contains(result, t => t.TipoEntrada == "VIP");
        }
        [Fact]
        public void Get_RetornaCorrectamente_Tarifa()
        {
            // Arrange
            var tarifaRepoMoq = new Mock<ITarifaRepository>();
            var funcionRepoMoq = new Mock<IFuncionRepository>();
            var tarifaService = new TarifaService(tarifaRepoMoq.Object, funcionRepoMoq.Object);

            var tarifa = new Tarifa { IdTarifa = 1, IdFuncion = 1, TipoEntrada = ETipoEntrada.General, Precio = 100m, Stock = 50 };
            var tarifaDTO = tarifa.Adapt<MostrarTarifaDTO>();
            tarifaRepoMoq.Setup(repo => repo.Select(1)).Returns(tarifa);

            // Act
            var result = tarifaService.Get(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100m, result.Precio);
            Assert.Equal("General", result.TipoEntrada);
        }
        [Fact]
        public void Post_CreaTarifaCorrectamente()
        {
            // Arrange
            var tarifaRepoMoq = new Mock<ITarifaRepository>();
            var funcionRepoMoq = new Mock<IFuncionRepository>();
            var tarifaService = new TarifaService(tarifaRepoMoq.Object, funcionRepoMoq.Object);

            var crearTarifaDto = new CrearTarifaDTO { IdFuncion = 1, TipoEntrada = ETipoEntrada.General, Precio = 100m, Stock = 50 };
            funcionRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(true);

            var tarifa = new Tarifa { IdTarifa = 1, IdFuncion = 1, TipoEntrada = ETipoEntrada.General, Precio = 100m, Stock = 50 };
            tarifaRepoMoq.Setup(repo => repo.Insert(It.IsAny<Tarifa>())).Returns(tarifa);

            // Act
            var result = tarifaService.Post(crearTarifaDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100m, result.Precio);
            Assert.Equal("General", result.TipoEntrada);
        }
        [Fact]
        public void Post_NoPuedeCrearTarifa_SiLaFuncionNoExiste()
        {
            // Arrange
            var tarifaRepoMoq = new Mock<ITarifaRepository>();
            var funcionRepoMoq = new Mock<IFuncionRepository>();
            var tarifaService = new TarifaService(tarifaRepoMoq.Object, funcionRepoMoq.Object);

            var crearTarifaDto = new CrearTarifaDTO { IdFuncion = 999, TipoEntrada = ETipoEntrada.General, Precio = 100m, Stock = 50 };
            funcionRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(false);

            // Act & Assert
            Assert.Throws<NotFoundException>(() => tarifaService.Post(crearTarifaDto));
        }
        [Fact]
public void Put_ActualizaTarifaCorrectamente()
{
            // Arrange
            var tarifaRepoMoq = new Mock<ITarifaRepository>();
            var funcionRepoMoq = new Mock<IFuncionRepository>();
            var tarifaService = new TarifaService(tarifaRepoMoq.Object, funcionRepoMoq.Object);

            var actualizarTarifaDto = new ActualizarTarifaDTO 
            { 
                Precio = 120m, 
                Stock = 60, 
                Estado = ETipoEstadoTarifa.Activa 
            };

            var tarifaActualizada = new Tarifa 
            {
                IdTarifa = 1,
                Precio = 120m,
                Stock = 60,
                Estado = ETipoEstadoTarifa.Activa
            };

            tarifaRepoMoq.Setup(r => r.Exists(It.IsAny<int>())).Returns(true);
            tarifaRepoMoq.Setup(r => r.Update(It.IsAny<Tarifa>(), It.IsAny<int>())).Returns(true);

            tarifaRepoMoq.Setup(r => r.Select(1)).Returns(tarifaActualizada);

            var result = tarifaService.Put(actualizarTarifaDto, 1);


            Assert.NotNull(result);
            Assert.Equal(120m, result.Precio);
            Assert.Equal(60, result.Stock);
            Assert.Equal("Activa", result.Estado);
        }

        [Fact]
        public void Put_NoPuedeActualizarTarifa_SiNoExiste()
        {

            var tarifaRepoMoq = new Mock<ITarifaRepository>();
            var funcionRepoMoq = new Mock<IFuncionRepository>();
            var tarifaService = new TarifaService(tarifaRepoMoq.Object, funcionRepoMoq.Object);

            var actualizarTarifaDto = new ActualizarTarifaDTO { Precio = 120m, Stock = 60, Estado = ETipoEstadoTarifa.Activa };
            tarifaRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(false);

            Assert.Throws<NotFoundException>(() => tarifaService.Put(actualizarTarifaDto, 999));
        }
    }
}
