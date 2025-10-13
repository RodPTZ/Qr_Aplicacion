namespace SistemaDeBoleteria.Core.Services;

public interface ILocalService
{
    IEnumerable<Local> GetLocales();
    Local GetLocalById(int id);
    void InsertLocal(Local local);
    void UpdateLocal(Local local);
    bool DeleteLocal(int id);
}
