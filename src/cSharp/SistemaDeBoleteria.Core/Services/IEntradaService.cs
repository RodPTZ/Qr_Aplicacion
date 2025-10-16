namespace SistemaDeBoleteria.Core.Services;

public interface IEntradaService
{
    IEnumerable<Entrada> GetEntradas();
    Entrada? GetEntradaById(int id);
    void AnularEntrada(int id);
}
