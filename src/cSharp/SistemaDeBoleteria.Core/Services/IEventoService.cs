using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;

namespace SistemaDeBoleteria.Core.Services;

public interface IEventoService
{
    IEnumerable<MostrarEventoDTO> GetEventos();
    MostrarEventoDTO? GetEventoById(int IdEvento);
    MostrarEventoDTO InsertEvento(CrearActualizarEventoDTO evento);
    MostrarEventoDTO UpdateEvento(CrearActualizarEventoDTO evento, int IdEvento);
    bool PublicarEvento(int IdEvento);
    bool CancelarEvento(int IdEvento);
}
