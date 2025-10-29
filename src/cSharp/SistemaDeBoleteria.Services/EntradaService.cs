using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.DTOs;
using System.Security.Cryptography.X509Certificates;
using SistemaDeBoleteria.Repositories;
using Mapster;
using SistemaDeBoleteria.Core.Models;
using System.Runtime.CompilerServices;

namespace SistemaDeBoleteria.Services
{
    public class EntradaService : IEntradaService
    {
        private readonly EntradaRepository entradaRepository = new EntradaRepository();

        public IEnumerable<MostrarEntradaDTO> GetAll() => entradaRepository.SelectAll().Adapt<IEnumerable<MostrarEntradaDTO>>();
        public MostrarEntradaDTO? GetById(int IdEntrada) =>
        entradaRepository.Select(IdEntrada) is null? null : entradaRepository.Select(IdEntrada).Adapt<MostrarEntradaDTO>();
        public bool AnularEntrada(int IdEntrada)
        {
            if(entradaRepository.Select(IdEntrada) is null)
                return false;
            return entradaRepository.UpdateEstado(IdEntrada);
            
        }
    }
}