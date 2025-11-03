using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Repositories;
using SistemaDeBoleteria.Core.DTOs;
using Mapster;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Services
{
    public class FuncionService : IFuncionService
    {
        public FuncionRepository funcionRepository = new FuncionRepository();

        public IEnumerable<MostrarFuncionDTO> GetAll() => funcionRepository.SelectAll().Adapt<IEnumerable<MostrarFuncionDTO>>();

        public MostrarFuncionDTO? Get(int idFuncion) => funcionRepository.Select(idFuncion).Adapt<MostrarFuncionDTO>();
        public MostrarFuncionDTO Post(CrearFuncionDTO funcion) => funcionRepository.Insert(funcion.Adapt<Funcion>()).Adapt<MostrarFuncionDTO>();
        public MostrarFuncionDTO Put(ActualizarFuncionDTO funcion, int idFuncion)
        {
            var funcionExiste = funcionRepository.Select(idFuncion);
            if (funcionExiste is null)
                return null!;
            // aqui deberia haber función de validación de negocio ()
            return  funcionRepository
                        .Update(funcion.Adapt<Funcion>(), idFuncion)
                        .Adapt<MostrarFuncionDTO>();
        }
        public void Cancelar(int idFuncion) => funcionRepository.UpdFuncionCancel(idFuncion);
    }
}