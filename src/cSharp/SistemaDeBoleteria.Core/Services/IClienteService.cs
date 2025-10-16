namespace SistemaDeBoleteria.Core.Services;

public interface IClienteService
{
    IEnumerable<Cliente> GetClientes();
    Cliente? GetClienteById(int id);
    void InsertCliente(Cliente cliente);
    void UpdateCliente(Cliente cliente);
}
