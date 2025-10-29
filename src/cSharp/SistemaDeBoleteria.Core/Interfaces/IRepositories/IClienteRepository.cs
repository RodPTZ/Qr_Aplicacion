using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface IClienteRepository
    {
        IEnumerable<Cliente> SelectAll();
        Cliente? Select(int id);
        Cliente Insert(Cliente cliente, Usuario usuario);
        Cliente Update(Cliente cliente, int idCliente);
    }
}