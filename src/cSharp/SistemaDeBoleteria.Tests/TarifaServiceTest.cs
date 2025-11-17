using Xunit;
using Moq;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Enums;
using System.Collections.Generic;

public class TarifaService
{
    [Fact]
    public void SelectAllByFuncionId_ReturnsTarifas()
    {
        var mock = new Mock<ITarifaRepository>();
        mock.Setup(r => r.SelectAllByFuncionId(1)).Returns(new List<Tarifa>
        {
            new Tarifa{ IdTarifa = 1, TipoEntrada = ETipoEntrada.General, Precio = 5000 },
            new Tarifa{ IdTarifa = 2, TipoEntrada = ETipoEntrada.VIP, Precio = 12000 }
        });

        var result = mock.Object.SelectAllByFuncionId(1);

        Assert.NotNull(result);
        Assert.Equal(2, ((List<Tarifa>)result).Count);
    }

    [Fact]
    public void Select_ReturnsTarifa()
    {
        var tarifa = new Tarifa
        {
            IdTarifa = 3,
            TipoEntrada = ETipoEntrada.Plus,
            Precio = 8000
        };

        var mock = new Mock<ITarifaRepository>();
        mock.Setup(r => r.Select(3)).Returns(tarifa);

        var result = mock.Object.Select(3);

        Assert.NotNull(result);
        Assert.Equal(ETipoEntrada.Plus, result.TipoEntrada);
    }

    [Fact]
    public void Insert_ReturnsInsertedTarifa()
    {
        var tarifa = new Tarifa
        {
            TipoEntrada = ETipoEntrada.General,
            Precio = 4000,
            Stock = 100
        };

        var mock = new Mock<ITarifaRepository>();
        mock.Setup(r => r.Insert(tarifa)).Returns(tarifa);

        var result = mock.Object.Insert(tarifa);

        Assert.NotNull(result);
        Assert.Equal(4000, result.Precio);
    }

    [Fact]
    public void Update_ReturnsUpdatedTarifa()
    {
        var tarifa = new Tarifa
        {
            TipoEntrada = ETipoEntrada.VIP,
            Precio = 15000
        };

        var mock = new Mock<ITarifaRepository>();
        mock.Setup(r => r.Update(tarifa, 5)).Returns(true);

        var result = mock.Object.Update(tarifa, 5);

        Assert.True(result);
    }
}
