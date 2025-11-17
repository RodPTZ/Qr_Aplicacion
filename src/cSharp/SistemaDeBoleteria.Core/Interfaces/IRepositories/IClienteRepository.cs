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
        Cliente Insert(Cliente cliente);
        bool Update(Cliente cliente, int idCliente);
        bool Exists(int idCliente);
    }
}