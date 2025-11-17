using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Exceptions;
using SistemaDeBoleteria.Services;
using Mapster;
using SistemaDeBoleteria.Core.Enums;
using MySqlConnector;

namespace SistemaDeBoleteria.Tests
{
    public class EventoXUnit
    {
        [Fact]
        public void GetAll_RetornaCorrectamente_Los_Eventos()
        {
            var eventoRepoMoq = new Mock<IEventoRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var eventoService = new EventoService(eventoRepoMoq.Object, localRepoMoq.Object);

            var eventos = new List<Evento>
            {
                new Evento { IdEvento = 1, IdLocal = 1, Nombre = "Concierto", Tipo = ETipoEvento.Música},
                new Evento { IdEvento = 2, IdLocal = 2, Nombre = "Teatro", Tipo = ETipoEvento.Teatro}
            };

            var eventosDTO = eventos.Adapt<IEnumerable<MostrarEventoDTO>>();
            eventoRepoMoq.Setup(repo => repo.SelectAll()).Returns(eventos);

            var result = eventoService.GetAll();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, e => e.Nombre == "Concierto");
            Assert.Contains(result, e => e.Nombre == "Teatro");
        }
        [Fact]
        public void GetById_RetornaCorrectamente_Evento()
        {
            // Arrange
            var eventoRepoMoq = new Mock<IEventoRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var eventoService = new EventoService(eventoRepoMoq.Object, localRepoMoq.Object);

            var evento = new Evento { IdEvento = 1, IdLocal = 1, Nombre = "Concierto A", Tipo = ETipoEvento.Música};
            var eventoDTO = evento.Adapt<MostrarEventoDTO>();
            eventoRepoMoq.Setup(repo => repo.Select(1)).Returns(evento);

            // Act
            var result = eventoService.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Concierto A", result.Nombre);
        }
        [Fact]
        public void Post_CreaEventoCorrectamente()
        {
            // Arrange
            var eventoRepoMoq = new Mock<IEventoRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var eventoService = new EventoService(eventoRepoMoq.Object, localRepoMoq.Object);

            var crearEventoDto = new CrearActualizarEventoDTO { IdLocal = 1, Nombre = "Concierto A", Tipo = ETipoEvento.Música };
            localRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(true);

            var evento = new Evento { IdEvento = 1, IdLocal = 1, Nombre = "Concierto A", Tipo = ETipoEvento.Música};
            eventoRepoMoq.Setup(repo => repo.Insert(It.IsAny<Evento>())).Returns(evento);

            // Act
            var result = eventoService.Post(crearEventoDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Concierto A", result.Nombre);
        }
        [Fact]
        public void Post_NoPuedeCrearEvento_SiElLocalNoExiste()
        {
            // Arrange
            var eventoRepoMoq = new Mock<IEventoRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var eventoService = new EventoService(eventoRepoMoq.Object, localRepoMoq.Object);

            var crearEventoDto = new CrearActualizarEventoDTO { IdLocal = 1, Nombre = "Concierto A", Tipo = ETipoEvento.Música };
            localRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(false);

            // Act & Assert
            Assert.Throws<NotFoundException>(() => eventoService.Post(crearEventoDto));
        }
        [Fact]
        public void Put_ActualizaEventoCorrectamente()
        {
            // Arrange
            var eventoRepoMoq = new Mock<IEventoRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var eventoService = new EventoService(eventoRepoMoq.Object, localRepoMoq.Object);

            var eventoDto = new CrearActualizarEventoDTO { IdLocal = 1, Nombre = "Concierto A", Tipo = ETipoEvento.Música };
            var evento = new Evento { IdEvento = 1, IdLocal = 1, Nombre = "Concierto A", Tipo = ETipoEvento.Música};

            eventoRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(true);
            eventoRepoMoq.Setup(repo => repo.Update(It.IsAny<Evento>(), It.IsAny<int>())).Returns(true);
            eventoRepoMoq.Setup(repo => repo.Select(It.IsAny<int>())).Returns(evento);

            // Act
            var result = eventoService.Put(eventoDto, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Concierto A", result.Nombre);
        }
        [Fact]
        public void PublicarEvento_PublcaCorrectamente()
        {
            // Arrange
            var eventoRepoMoq = new Mock<IEventoRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var eventoService = new EventoService(eventoRepoMoq.Object, localRepoMoq.Object);

            eventoRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(true);
            eventoRepoMoq.Setup(repo => repo.HasFunciones(It.IsAny<int>())).Returns(true);
            eventoRepoMoq.Setup(repo => repo.HasTarifasActivas(It.IsAny<int>())).Returns(true);
            eventoRepoMoq.Setup(repo => repo.UpdEstadoPublic(It.IsAny<int>())).Verifiable();

            // Act
            eventoService.PublicarEvento(1);

            // Assert
            eventoRepoMoq.Verify(repo => repo.UpdEstadoPublic(1), Times.Once());
        }
        [Fact]
        public void PublicarEvento_NoPuedePublicar_SiNoTieneFunciones()
        {
            // Arrange
            var eventoRepoMoq = new Mock<IEventoRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var eventoService = new EventoService(eventoRepoMoq.Object, localRepoMoq.Object);

            eventoRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(true);
            eventoRepoMoq.Setup(repo => repo.HasFunciones(It.IsAny<int>())).Returns(false);

            // Act & Assert
            Assert.Throws<NoContentException>(() => eventoService.PublicarEvento(1));
        }
        [Fact]
        public void CancelarEvento_CancelaCorrectamente()
        {
            // Arrange
            var eventoRepoMoq = new Mock<IEventoRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var eventoService = new EventoService(eventoRepoMoq.Object, localRepoMoq.Object);

            eventoRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(true);
            eventoRepoMoq.Setup(repo => repo.UpdEstadoCancel(It.IsAny<int>())).Verifiable();

            // Act
            eventoService.CancelarEvento(1);

            // Assert
            eventoRepoMoq.Verify(repo => repo.UpdEstadoCancel(1), Times.Once());
        }
    }
}
