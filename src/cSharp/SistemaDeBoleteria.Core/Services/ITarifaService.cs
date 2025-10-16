namespace SistemaDeBoleteria.Core.Services;

public interface ITarifaService
{
    void InsertTarifa(Tarifa tarifa);
    IEnumerable<Tarifa> GetTarifasByFuncionId(int idFuncion);
    Tarifa? GetTarifaById(int id);
    void UpdateTarifa(Tarifa tarifa, int IdTarifa);
}
