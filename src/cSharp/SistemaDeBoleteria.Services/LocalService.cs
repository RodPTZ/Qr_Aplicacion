using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Exceptions;
using SistemaDeBoleteria.Repositories;
using Mapster;
using System.Net.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeBoleteria.Services
{
    public class LocalService : ILocalService
    {
        private readonly ILocalRepository localRepository;
        public LocalService(ILocalRepository localRepository)
        {
            this.localRepository = localRepository;
        }
        public IEnumerable<MostrarLocalDTO> GetAll() 
        => localRepository
                    .SelectAll()
                    .Adapt<IEnumerable<MostrarLocalDTO>>();
        public MostrarLocalDTO? Get(int idLocal)
        => localRepository
                    .Select(idLocal)
                    .Adapt<MostrarLocalDTO>();
        public MostrarLocalDTO Post(CrearActualizarLocalDTO local)
        => localRepository
                    .Insert(local.Adapt<Local>())
                    .Adapt<MostrarLocalDTO>();
        public MostrarLocalDTO Put(CrearActualizarLocalDTO local, int idLocal) 
        {
            if(!localRepository.Exists(idLocal))
                throw new NotFoundException("No se encontró el local especificado para actualizar.");
            if(!localRepository.Update(local.Adapt<Local>(), idLocal))
                throw new DataBaseException("No se pudo actualizar el local especificado.");

            return localRepository
                    .Select(idLocal)
                    .Adapt<MostrarLocalDTO>();
        }
        public void Delete(int idLocal)
        {
            if(!localRepository.Exists(idLocal))
                throw new NotFoundException("No se encontró el local especificado.");
            if(localRepository.HasFunciones(idLocal))
                throw new BusinessException("No se puede eliminar el local porque tiene funciones asociadas.");
            if(localRepository.HasEventos(idLocal))
                throw new BusinessException("No se puede eliminar el local porque tiene eventos asociadas.");

            if(!localRepository.Delete(idLocal))
                throw new DataBaseException("No se puede eliminar un local que ya fue eliminado.");
        }
    }
}