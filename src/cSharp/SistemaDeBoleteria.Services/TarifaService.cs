using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Repositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using Mapster;

namespace SistemaDeBoleteria.Services
{
    public class TarifaService : ITarifaService
    {
        private readonly TarifaRepository tarifaRepository = new TarifaRepository();
        
        public IEnumerable<MostrarTarifaDTO> GetAllByFuncionId(int IdFuncion)
        => tarifaRepository
                .SelectAllByFuncionId(IdFuncion)
                .Adapt<IEnumerable<MostrarTarifaDTO>>();

        public MostrarTarifaDTO? Get(int IdTarifa)
        => tarifaRepository
                .Select(IdTarifa)
                .Adapt<MostrarTarifaDTO>();

        public MostrarTarifaDTO Post(CrearTarifaDTO tarifa)
        => tarifaRepository
                    .Insert(tarifa.Adapt<Tarifa>())
                    .Adapt<MostrarTarifaDTO>();
                    
        public MostrarTarifaDTO Put(ActualizarTarifaDTO tarifa, int IdTarifa) 
        => tarifaRepository
                    .Update(tarifa.Adapt<Tarifa>(), IdTarifa)
                    .Adapt<MostrarTarifaDTO>();
    }
}