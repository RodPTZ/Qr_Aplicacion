namespace SistemaDeBoleteria.Core.Services;

public interface IEventoService
{
    IEnumerable<Evento> GetEventos();
    Evento? GetEventoById(int IdEvento);
    void InsertEvento(Evento evento);
    void UpdateEvento(Evento evento);
    bool PublicarEvento(int IdEvento);
    bool CancelarEvento(int IdEvento);
}
