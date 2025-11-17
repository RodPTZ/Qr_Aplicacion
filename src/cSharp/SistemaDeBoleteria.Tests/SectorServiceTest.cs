using SistemaDeBoleteria.Services;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Exceptions;
using SistemaDeBoleteria.Core.Validations;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace SistemaDeBoleteria.Test.Services
{
    public class SectorServiceTests
    {
        private readonly Mock<ISectorRepository> sectorRepositoryMock;
        private readonly Mock<ILocalRepository> localRepositoryMock;
        private readonly SectorService sectorService;
        private readonly SectorValidator validator;

        public SectorServiceTests()
        {
            sectorRepositoryMock = new Mock<ISectorRepository>();
            localRepositoryMock = new Mock<ILocalRepository>();
            sectorService = new SectorService(sectorRepositoryMock.Object, localRepositoryMock.Object);
            validator = new SectorValidator();
        }

        // -----------------------------------------------------------------------
        // GET ALL BY LOCAL ID
        // -----------------------------------------------------------------------

        [Fact]
        public void CuandoObtengaSectoresPorIdLocal_DebeRetornarListaDeSectores()
        {
            // Arrange
            int idLocal = 1;
            var sectores = new List<Sector>
            {
                new Sector { IdSector = 10, IdLocal = idLocal, Capacidad = 50 },
                new Sector { IdSector = 11, IdLocal = idLocal, Capacidad = 80 }
            };

            sectorRepositoryMock
                .Setup(r => r.SelectAllByLocalId(idLocal))
                .Returns(sectores);

            // Act
            var resultado = sectorService.GetAllByLocalId(idLocal);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(sectores.Count, resultado.Count());

            sectorRepositoryMock.Verify(r => r.SelectAllByLocalId(idLocal), Times.Once);
        }

        // -----------------------------------------------------------------------
        // POST
        // -----------------------------------------------------------------------

        [Fact]
        public void CuandoCreeSector_DebeRetornarSectorMostrado()
        {
            // Arrange
            var idLocal = 1;
            var dto = new CrearActualizarSectorDTO { Capacidad = 30 };
            var sector = new Sector { IdSector = 5, IdLocal = idLocal, Capacidad = dto.Capacidad };

            localRepositoryMock
                .Setup(r => r.Exists(idLocal))
                .Returns(true);

            sectorRepositoryMock
                .Setup(r => r.Insert(It.Is<Sector>(s => s.Capacidad == dto.Capacidad), idLocal))
                .Returns(sector);

            // Act
            var resultado = sectorService.Post(dto, idLocal);

            // Assert
            Assert.Equal(sector.IdSector, resultado.IdSector);
            Assert.Equal(sector.IdLocal, resultado.IdLocal);
            Assert.Equal(sector.Capacidad, resultado.Capacidad);

            localRepositoryMock.Verify(r => r.Exists(idLocal), Times.Once);
            sectorRepositoryMock.Verify(r => r.Insert(It.IsAny<Sector>(), idLocal), Times.Once);
        }

        [Fact]
        public void CuandoCreeSectorSinLocal_DebeLanzarNotFoundException()
        {
            // Arrange
            var idLocal = 1;
            var dto = new CrearActualizarSectorDTO { Capacidad = 10 };

            localRepositoryMock
                .Setup(r => r.Exists(idLocal))
                .Returns(false);

            // Act Assert
            Assert.Throws<NotFoundException>(() => sectorService.Post(dto, idLocal));

            localRepositoryMock.Verify(r => r.Exists(idLocal), Times.Once);
            sectorRepositoryMock.Verify(r => r.Insert(It.IsAny<Sector>(), It.IsAny<int>()), Times.Never);
        }

        // -----------------------------------------------------------------------
        // PUT
        // -----------------------------------------------------------------------

        [Fact]
        public void CuandoActualiceSector_DebeRetornarSectorActualizado()
        {
            // Arrange
            int idSector = 3;
            var dto = new CrearActualizarSectorDTO { Capacidad = 45 };

            var sectorActualizado = new Sector { IdSector = idSector, IdLocal = 2, Capacidad = dto.Capacidad };

            sectorRepositoryMock
                .Setup(r => r.Exists(idSector))
                .Returns(true);

            sectorRepositoryMock
                .Setup(r => r.Update(It.Is<Sector>(s => s.Capacidad == dto.Capacidad), idSector))
                .Returns(true);

            sectorRepositoryMock
                .Setup(r => r.Select(idSector))
                .Returns(sectorActualizado);

            // Act
            var resultado = sectorService.Put(dto, idSector);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(sectorActualizado.IdSector, resultado.IdSector);
            Assert.Equal(sectorActualizado.IdLocal, resultado.IdLocal);
            Assert.Equal(sectorActualizado.Capacidad, resultado.Capacidad);

            sectorRepositoryMock.Verify(r => r.Exists(idSector), Times.Once);
            sectorRepositoryMock.Verify(r => r.Update(It.IsAny<Sector>(), idSector), Times.Once);
            sectorRepositoryMock.Verify(r => r.Select(idSector), Times.Once);
        }

        [Fact]
        public void CuandoActualiceSectorInexistente_DebeLanzarNotFoundException()
        {
            // Arrange
            int idSector = 3;
            var dto = new CrearActualizarSectorDTO { Capacidad = 10 };

            sectorRepositoryMock
                .Setup(r => r.Exists(idSector))
                .Returns(false);

            // Act Assert
            Assert.Throws<NotFoundException>(() => sectorService.Put(dto, idSector));

            sectorRepositoryMock.Verify(r => r.Exists(idSector), Times.Once);
            sectorRepositoryMock.Verify(r => r.Update(It.IsAny<Sector>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void CuandoUpdateFalle_DebeLanzarBusinessException()
        {
            // Arrange
            int idSector = 3;
            var dto = new CrearActualizarSectorDTO { Capacidad = 10 };

            sectorRepositoryMock
                .Setup(r => r.Exists(idSector))
                .Returns(true);

            sectorRepositoryMock
                .Setup(r => r.Update(It.IsAny<Sector>(), idSector))
                .Returns(false);

            // Act Assert
            Assert.Throws<BusinessException>(() => sectorService.Put(dto, idSector));

            sectorRepositoryMock.Verify(r => r.Exists(idSector), Times.Once);
            sectorRepositoryMock.Verify(r => r.Update(It.IsAny<Sector>(), idSector), Times.Once);
        }

        // -----------------------------------------------------------------------
        // DELETE
        // -----------------------------------------------------------------------

        [Fact]
        public void CuandoElimineSector_DebeEliminarCorrectamente()
        {
            // Arrange
            int idSector = 7;

            sectorRepositoryMock.Setup(r => r.Exists(idSector)).Returns(true);
            sectorRepositoryMock.Setup(r => r.HasFunciones(idSector)).Returns(false);
            sectorRepositoryMock.Setup(r => r.Delete(idSector)).Returns(true);

            // Act
            sectorService.Delete(idSector);

            // Assert
            sectorRepositoryMock.Verify(r => r.Exists(idSector), Times.Once);
            sectorRepositoryMock.Verify(r => r.HasFunciones(idSector), Times.Once);
            sectorRepositoryMock.Verify(r => r.Delete(idSector), Times.Once);
        }

        [Fact]
        public void CuandoElimineSectorInexistente_DebeLanzarNotFoundException()
        {
            // Arrange
            int idSector = 7;

            sectorRepositoryMock.Setup(r => r.Exists(idSector)).Returns(false);

            // Act Assert
            Assert.Throws<NotFoundException>(() => sectorService.Delete(idSector));

            sectorRepositoryMock.Verify(r => r.Exists(idSector), Times.Once);
            sectorRepositoryMock.Verify(r => r.HasFunciones(It.IsAny<int>()), Times.Never);
            sectorRepositoryMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void CuandoElimineSectorConFunciones_DebeLanzarBusinessException()
        {
            // Arrange
            int idSector = 7;

            sectorRepositoryMock.Setup(r => r.Exists(idSector)).Returns(true);
            sectorRepositoryMock.Setup(r => r.HasFunciones(idSector)).Returns(true);

            // Act Assert
            Assert.Throws<BusinessException>(() => sectorService.Delete(idSector));

            sectorRepositoryMock.Verify(r => r.Exists(idSector), Times.Once);
            sectorRepositoryMock.Verify(r => r.HasFunciones(idSector), Times.Once);
            sectorRepositoryMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void CuandoDeleteFalle_DebeLanzarNotFoundException()
        {
            // Arrange
            int idSector = 7;

            sectorRepositoryMock.Setup(r => r.Exists(idSector)).Returns(true);
            sectorRepositoryMock.Setup(r => r.HasFunciones(idSector)).Returns(false);
            sectorRepositoryMock.Setup(r => r.Delete(idSector)).Returns(false);

            // Act Assert
            Assert.Throws<NotFoundException>(() => sectorService.Delete(idSector));

            sectorRepositoryMock.Verify(r => r.Exists(idSector), Times.Once);
            sectorRepositoryMock.Verify(r => r.HasFunciones(idSector), Times.Once);
            sectorRepositoryMock.Verify(r => r.Delete(idSector), Times.Once);
        }

        // -----------------------------------------------------------------------
        // VALIDATOR
        // -----------------------------------------------------------------------

        [Fact]
        public void CuandoCapacidadSeaValida_DebeSerCorrecto()
        {
            var dto = new CrearActualizarSectorDTO { Capacidad = 5 };

            var resultado = validator.TestValidate(dto);

            resultado.ShouldNotHaveValidationErrorFor(s => s.Capacidad);
        }

        [Fact]
        public void CuandoCapacidadSeaCeroODebajo_DebeDarError()
        {
            var dto = new CrearActualizarSectorDTO { Capacidad = 0 };

            var resultado = validator.TestValidate(dto);

            resultado.ShouldHaveValidationErrorFor(s => s.Capacidad);
        }
    }
}
