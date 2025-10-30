using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;

namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface IEventoService
{
    IEnumerable<MostrarEventoDTO> GetAll();
    MostrarEventoDTO? GetById(int IdEvento);
    MostrarEventoDTO Post(CrearActualizarEventoDTO evento);
    MostrarEventoDTO? Put(CrearActualizarEventoDTO evento, int IdEvento);
    (byte secuencia, string ErrorMessage) PublicarEvento(int IdEvento);
    bool CancelarEvento(int IdEvento);
}
