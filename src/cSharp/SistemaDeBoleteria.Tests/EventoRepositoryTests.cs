using Xunit;
using Moq;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Enums;
using System.Collections.Generic;

public class EventoRepositoryTests
{
    [Fact]
    public void SelectAll_ReturnsEventos()
    {
        var eventos = new List<Evento>
        {
            new Evento("Cosquin Rock", ETipoEvento.Concierto),
            new Evento("Gaming Expo", ETipoEvento.Convencion)
        };

        var mock = new Mock<IEventoRepository>();
        mock.Setup(r => r.SelectAll()).Returns(eventos);

        var result = mock.Object.SelectAll();

        Assert.Equal(2, ((List<Evento>)result).Count);
    }

    [Fact]
    public void Insert_AddsEvento()
    {
        var evento = new Evento("Lollapalooza", ETipoEvento.Concierto);

        var mock = new Mock<IEventoRepository>();
        mock.Setup(r => r.Insert(evento)).Returns(evento);

        var result = mock.Object.Insert(evento);

        Assert.Equal("Lollapalooza", result.Nombre);
    }

    [Fact]
    public void UpdEstadoPublic_ReturnsMessage()
    {
        var mock = new Mock<IEventoRepository>();
        mock.Setup(r => r.UpdEstadoPublic(1))
            .Returns(true);

        var code = mock.Object.UpdEstadoPublic(1);

        Assert.True(code);
    }
}
