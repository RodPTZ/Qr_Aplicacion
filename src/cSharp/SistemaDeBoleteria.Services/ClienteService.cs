using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Repositories;
using Mapster;
using SistemaDeBoleteria.Core.Validations;
using System.Security.Cryptography.X509Certificates;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Services
{
    public class ClienteService : IClienteService
    {
        private readonly ClienteRepository clienteRepository = new ClienteRepository();
        public IEnumerable<MostrarClienteDTO> GetAll() => clienteRepository.SelectAll().Adapt<IEnumerable<MostrarClienteDTO>>();
        public MostrarClienteDTO? GetById(int id) => clienteRepository.Select(id).Adapt<MostrarClienteDTO>();
        public MostrarClienteDTO Post(CrearClienteDTO cliente) => clienteRepository.Insert(cliente.Adapt<Cliente>(), cliente.Adapt<Usuario>()).Adapt<MostrarClienteDTO>(); 
        public MostrarClienteDTO? Put(ActualizarClienteDTO cliente, int IdCliente)
        {
            var clienteExiste = clienteRepository.Select(IdCliente);
            if (clienteExiste is null)
                return null;

            return clienteRepository.Update(cliente.Adapt<Cliente>(), IdCliente).Adapt<MostrarClienteDTO>();
        }
    }
}