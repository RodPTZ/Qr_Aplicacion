using Xunit;
using Moq;
using System.Collections.Generic;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Services;
using Mapster;
using SistemaDeBoleteria.Core.Exceptions;

namespace SistemaDeBoleteria.Tests
{
    public class SectorServiceXUnit
    {
        [Fact]
        public void GetAllByLocalId_RetornaCorrectamente_Los_Sectores()
        {
            
            var sectorRepoMoq = new Mock<ISectorRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var sectorService = new SectorService(sectorRepoMoq.Object, localRepoMoq.Object);

            
            var sectores = new List<Sector>
            {
                new Sector { IdSector = 1, IdLocal = 1, Capacidad = 100 },
                new Sector { IdSector = 2, IdLocal = 1, Capacidad = 150 }
            };

            var sectoresDTO = sectores.Adapt<IEnumerable<MostrarSectorDTO>>();

            sectorRepoMoq.Setup(repo => repo.SelectAllByLocalId(1)).Returns(sectores);

           
            var result = sectorService.GetAllByLocalId(1);

            
            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.Capacidad == 100);
            Assert.Contains(result, s => s.Capacidad == 150);
        }
        [Fact]
        public void Post_CreaSectorCorrectamente()
        {
            // Arrange
            var sectorRepoMoq = new Mock<ISectorRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var sectorService = new SectorService(sectorRepoMoq.Object, localRepoMoq.Object);
            var crearSectorDto = new CrearActualizarSectorDTO { Capacidad = 200 };

            localRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(true);

            var nuevoSector = new Sector { IdSector = 1, IdLocal = 1, Capacidad = 200 };

            sectorRepoMoq.Setup(repo => repo.Insert(It.IsAny<Sector>(), It.IsAny<int>())).Returns(nuevoSector);

      
            var result = sectorService.Post(crearSectorDto, 1);

      
            Assert.NotNull(result);
            Assert.Equal(200, result.Capacidad);
            Assert.Equal(1, result.IdLocal);
        }
        [Fact]
        public void Put_ActualizaSectorCorrectamente()
        {
            // Arrange
            var sectorRepoMoq = new Mock<ISectorRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var sectorService = new SectorService(sectorRepoMoq.Object, localRepoMoq.Object);
            var actualizarSectorDto = new CrearActualizarSectorDTO { Capacidad = 250 };

            sectorRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(true);
            sectorRepoMoq.Setup(repo => repo.Update(It.IsAny<Sector>(), It.IsAny<int>())).Returns(true);

            var sector = new Sector { IdSector = 1, IdLocal = 1, Capacidad = 250 };
            sectorRepoMoq.Setup(repo => repo.Select(It.IsAny<int>())).Returns(sector);

            var result = sectorService.Put(actualizarSectorDto, 1);

            Assert.NotNull(result);
            Assert.Equal(250, result.Capacidad);
        }

        [Fact]
        public void Delete_EliminaSectorCorrectamente()
        {
        
            var sectorRepoMoq = new Mock<ISectorRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var sectorService = new SectorService(sectorRepoMoq.Object, localRepoMoq.Object);

            sectorRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(true);
            sectorRepoMoq.Setup(repo => repo.HasFunciones(It.IsAny<int>())).Returns(false);
            sectorRepoMoq.Setup(repo => repo.Delete(It.IsAny<int>())).Returns(true);

            sectorService.Delete(1);
            sectorRepoMoq.Verify(repo => repo.Delete(1), Times.Once());
        }

        [Fact]
        public void Delete_NoSePuedeEliminarConFunciones()
        {
            // Arrange
            var sectorRepoMoq = new Mock<ISectorRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var sectorService = new SectorService(sectorRepoMoq.Object, localRepoMoq.Object);

            sectorRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(true);
            sectorRepoMoq.Setup(repo => repo.HasFunciones(It.IsAny<int>())).Returns(true);

            // Act & Assert
            Assert.Throws<BusinessException>(() => sectorService.Delete(1));
        }

        [Fact]
        public void Post_NoSePuedeCrearSector_SiElLocalNoExiste()
        {
            // Arrange
            var sectorRepoMoq = new Mock<ISectorRepository>();
            var localRepoMoq = new Mock<ILocalRepository>();
            var sectorService = new SectorService(sectorRepoMoq.Object, localRepoMoq.Object);
            var crearSectorDto = new CrearActualizarSectorDTO { Capacidad = 200 };

            localRepoMoq.Setup(repo => repo.Exists(It.IsAny<int>())).Returns(false);

            // Act & Assert
            Assert.Throws<NotFoundException>(() => sectorService.Post(crearSectorDto, 1));
        }
    }
}
