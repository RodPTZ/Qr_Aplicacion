using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.DTOs;
using System.Security.Cryptography.X509Certificates;
using SistemaDeBoleteria.Repositories;
using Mapster;

namespace SistemaDeBoleteria.Services
{
    public class EntradaService : IEntradaService
    {
        private readonly EntradaRepository entradaRepository = new EntradaRepository();

        public IEnumerable<MostrarEntradaDTO> GetAll() => entradaRepository.SelectAll().Adapt<IEnumerable<MostrarEntradaDTO>>();
        public MostrarEntradaDTO? GetById(int IdEntrada) => entradaRepository.Select(IdEntrada).Adapt<MostrarEntradaDTO>();
        public void AnularEntrada(int IdEntrada) => entradaRepository.UpdateEstado(IdEntrada);
    }
}