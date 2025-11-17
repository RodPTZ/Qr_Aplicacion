using System;
using Xunit;
using Moq;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Services;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;

namespace SistemaDeBoleteria.Tests
{
    public class CodigoQRXUnit
    {
        private readonly Mock<ICodigoQRRepository> mockRepo;
        private readonly CodigoQRService service;

        public CodigoQRXUnit()
        {
            mockRepo = new Mock<ICodigoQRRepository>();
            service = new CodigoQRService(mockRepo.Object);
        }
        [Fact]
        public void ValidateQR_QRNoExiste_DevuelveNoExiste()
        {
            mockRepo.Setup(r => r.Exists(1, "ABC")).Returns(false);

            var resultado = service.ValidateQR(1, "ABC");

            Assert.Equal("NoExiste", resultado);
        }
        [Fact]
        public void ValidateQR_DatosIncompletos_DevuelveFirmaInvalida()
        {
            mockRepo.Setup(r => r.Exists(1, "ABC")).Returns(true);
            mockRepo.Setup(r => r.SelectData(1))
                    .Returns((null!, null!, null!)); // datos incompletos

            var resultado = service.ValidateQR(1, "ABC");

            Assert.Equal("FirmaInvalida", resultado);
        }
        [Fact]
        public void ValidateQR_EntradaAnulada_DevuelveFirmaInvalida()
        {
            mockRepo.Setup(r => r.Exists(1, "ABC")).Returns(true);

            var entrada = new Entrada { Estado = ETipoEstadoEntrada.Anulado };
            var funcion = new Funcion { Fecha = DateOnly.FromDateTime(DateTime.Now) };
            var qr = new CodigoQR { TipoEstado = ETipoEstadoQR.NoExiste };

            mockRepo.Setup(r => r.SelectData(1))
                    .Returns((entrada, funcion, qr));

            var resultado = service.ValidateQR(1, "ABC");

            Assert.Equal("FirmaInvalida", resultado);
        }
        [Fact]
        public void ValidateQR_PrimeraValidacion_DevuelveOk()
        {
            mockRepo.Setup(r => r.Exists(1, "ABC")).Returns(true);

            var entrada = new Entrada { Estado = ETipoEstadoEntrada.Pagado };

            var funcion = new Funcion
            {
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                AperturaTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(-1)),
                CierreTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2))
            };

            var qr = new CodigoQR { TipoEstado = ETipoEstadoQR.NoExiste };

            mockRepo.Setup(r => r.SelectData(1)).Returns((entrada, funcion, qr));
            mockRepo.Setup(r => r.UpdateEstado(1, ETipoEstadoQR.Ok))
                    .Returns(ETipoEstadoQR.Ok);

            var resultado = service.ValidateQR(1, "ABC");

            Assert.Equal("Ok", resultado);
        }

        [Fact]
        public void ValidateQR_QRYaUsado_DevuelveYaUsada()
        {
            mockRepo.Setup(r => r.Exists(1, "ABC")).Returns(true);

            var entrada = new Entrada { Estado = ETipoEstadoEntrada.Pagado };

            var funcion = new Funcion
            {
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                AperturaTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(-1)),
                CierreTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2))
            };

            var qr = new CodigoQR { TipoEstado = ETipoEstadoQR.Ok };

            mockRepo.Setup(r => r.SelectData(1)).Returns((entrada, funcion, qr));
            mockRepo.Setup(r => r.UpdateEstado(1, ETipoEstadoQR.YaUsada))
                    .Returns(ETipoEstadoQR.YaUsada);

            var resultado = service.ValidateQR(1, "ABC");

            Assert.Equal("YaUsada", resultado);
        }
    }
}
