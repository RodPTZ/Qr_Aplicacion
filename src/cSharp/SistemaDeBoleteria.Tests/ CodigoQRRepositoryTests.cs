using Xunit;
using Moq;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Services;

public class CodigoQRRepositoryTests
{
    [Fact]
    public void CuandoGetQRByEntradaIdYExiste_DebeRetornarBytes()
    {
        // Arrange
        var repoMock = new Mock<ICodigoQRRepository>();

        var idEntrada = 10;
        var codigo = "ABC123";

        var qr = new CodigoQR(idEntrada, codigo)
        {
            IdQR = 1,
            TipoEstado = ETipoEstadoQR.NoExiste
        };

        // simulamos que el QR existe
        repoMock
            .Setup(r => r.SelectById(idEntrada))
            .Returns(qr);

        var service = new CodigoQRService(repoMock.Object);

        // Act
        var resultado = service.GetQRByEntradaId(idEntrada);

        // Assert
        Assert.NotNull(resultado);
        Assert.True(resultado.Length > 0);

        repoMock.Verify(r => r.SelectById(idEntrada), Times.Once);
    }

    [Fact]
    public void CuandoGetQRByEntradaIdYNoExiste_DebeRetornarNull()
    {
        // Arrange
        var repoMock = new Mock<ICodigoQRRepository>();

        var idEntrada = 99;

        repoMock
            .Setup(r => r.SelectById(idEntrada))
            .Returns((CodigoQR)null);

        var service = new CodigoQRService(repoMock.Object);

        // Act
        var resultado = service.GetQRByEntradaId(idEntrada);

        // Assert
        Assert.Null(resultado);

        repoMock.Verify(r => r.SelectById(idEntrada), Times.Once);
    }

    [Fact]
    public void CuandoValidateQRNoExiste_DebeRetornarMensajeError()
    {
        // Arrange
        var repoMock = new Mock<ICodigoQRRepository>();

        var idEntrada = 12;
        var codigo = "XXXXXX";

        repoMock
            .Setup(r => r.Exists(idEntrada, codigo))
            .Returns(false);

        var service = new CodigoQRService(repoMock.Object);

        // Act
        var resultado = service.ValidateQR(idEntrada, codigo);

        // Assert
        Assert.Contains("noexiste", resultado.ToLower());

        repoMock.Verify(r => r.Exists(idEntrada, codigo), Times.Once);
        repoMock.Verify(r => r.SelectData(It.IsAny<int>()), Times.Never);
        repoMock.Verify(r => r.UpdateEstado(It.IsAny<int>(), It.IsAny<ETipoEstadoQR>()), Times.Never);
    }

    [Fact]
    public void CuandoValidateQRYaUsado_DebeRetornarMensajeAdvertenciaYNoActualizar()
    {
        // Arrange
        var repoMock = new Mock<ICodigoQRRepository>();

        var idEntrada = 8;
        var codigo = "COD789";

        var entrada = new Entrada();
        var funcion = new Funcion();
        var qr = new CodigoQR(idEntrada, codigo)
        {
            TipoEstado = ETipoEstadoQR.YaUsada
        };

        repoMock
            .Setup(r => r.Exists(idEntrada, codigo))
            .Returns(true);

        repoMock
            .Setup(r => r.SelectData(idEntrada))
            .Returns((entrada, funcion, qr));

        var service = new CodigoQRService(repoMock.Object);

        // Act
        var resultado = service.ValidateQR(idEntrada, codigo);

        // Assert
        Assert.Contains("expirada", resultado.ToLower());

        repoMock.Verify(r => r.Exists(idEntrada, codigo), Times.Once);
        repoMock.Verify(r => r.SelectData(idEntrada), Times.Once);
        repoMock.Verify(r => r.UpdateEstado(It.IsAny<int>(), It.IsAny<ETipoEstadoQR>()), Times.Never);
    }
}
