namespace SistemaDeBoleteria.Core.Services;

public interface IOrdenService
{
    IEnumerable<Orden> GetOrdenes();
    Orden? GetOrdenById(int idOrden);
    void InsertOrden(Orden orden);
    void PagarOrden(int idOrden);
    void CancelarOrden(int idOrden);
}
