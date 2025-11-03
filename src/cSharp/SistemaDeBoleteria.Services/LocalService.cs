using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Repositories;
using SistemaDeBoleteria.Core.Models;
using Mapster;

namespace SistemaDeBoleteria.Services
{
    public class LocalService : ILocalService
    {
        private readonly LocalRepository localRepository = new LocalRepository();
        public IEnumerable<MostrarLocalDTO> GetAll() => localRepository.SelectAll().Adapt<IEnumerable<MostrarLocalDTO>>();
        public MostrarLocalDTO? Get(int idLocal) => localRepository.Select(idLocal).Adapt<MostrarLocalDTO>();
        public MostrarLocalDTO Post(CrearActualizarLocalDTO local) => localRepository.Insert(local.Adapt<Local>()).Adapt<MostrarLocalDTO>();
        public MostrarLocalDTO Put(CrearActualizarLocalDTO local, int idLocal) => localRepository.Update(local.Adapt<Local>(), idLocal).Adapt<MostrarLocalDTO>();
        public bool Delete(int idLocal) => localRepository.Delete(idLocal);
    }
}