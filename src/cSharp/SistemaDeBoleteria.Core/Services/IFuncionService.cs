namespace SistemaDeBoleteria.Core.Services;

public interface IFuncionService
{
    void InsertFuncion(Funcion funcion);
    IEnumerable<Funcion> GetFunciones();
    Funcion? GetFuncionById(int id);
    void UpdateFuncion(Funcion funcion, int IdFuncion);
    void CancelarFuncion(int id);
}
