using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.Exceptions;
using Mapster;

namespace SistemaDeBoleteria.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository clienteRepository;
        private readonly ILoginRepository loginRepository;
        public ClienteService(IClienteRepository clienteRepository, ILoginRepository loginRepository)
        {
            this.clienteRepository = clienteRepository;
            this.loginRepository = loginRepository; 
        }
        public IEnumerable<MostrarClienteDTO> GetAll() 
        => clienteRepository
                .SelectAll()
                .Adapt<IEnumerable<MostrarClienteDTO>>();
        public MostrarClienteDTO? GetById(int idCliente) 
        => clienteRepository
                .Select(idCliente)
                .Adapt<MostrarClienteDTO>();
        public MostrarClienteDTO Post(CrearClienteDTO cliente)
        {
            var _usuario = cliente.Adapt<Usuario>();
            var _cliente = cliente.Adapt<Cliente>();

            _usuario.Rol = ERolUsuario.Cliente;
            
            _cliente.IdUsuario = loginRepository.Insert(_usuario).IdUsuario;
                             
            return clienteRepository
                        .Insert(_cliente)
                        .Adapt<MostrarClienteDTO>();
        }
        public MostrarClienteDTO? Put(ActualizarClienteDTO cliente, int idCliente)
        {
            if(!clienteRepository.Exists(idCliente))
                throw new NotFoundException("No se encontró al cliente especificado.");
            if(!clienteRepository.Update(cliente.Adapt<Cliente>(), idCliente))
                throw new BusinessException("No se pudo actualizar el cliente especificado.");

            return clienteRepository
                        .Select(idCliente)
                        .Adapt<MostrarClienteDTO>();
        }
    }
}