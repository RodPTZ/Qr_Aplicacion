using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.Exceptions;
using Mapster;
using MySqlConnector;


namespace SistemaDeBoleteria.Services
{
    public class EntradaService : IEntradaService
    {
        private readonly IEntradaRepository entradaRepository;
        public EntradaService(IEntradaRepository entradaRepository)
        {
            this.entradaRepository = entradaRepository;
        }

        public IEnumerable<MostrarEntradaDTO> GetAll()
        => entradaRepository
                .SelectAll()
                .Adapt<IEnumerable<MostrarEntradaDTO>>();
        public MostrarEntradaDTO? GetById(int IdEntrada)
        => entradaRepository
                .Select(IdEntrada)
                .Adapt<MostrarEntradaDTO>();
        public bool AnularEntrada(int idEntrada)
        {
            var entrada = entradaRepository.Select(idEntrada);
            if(entrada is null)
                throw new NotFoundException("No se encontró la entrada especificada.");
            if(entrada.Estado == ETipoEstadoEntrada.Anulado)
                throw new BusinessException("No se puede anular una entrada que se encuentra anulada.");
            
            try
            {
                return entradaRepository.UpdateEstado(idEntrada);
            }
            catch (MySqlException ex)
            {
                throw new DataBaseException(ex.Message);
            }
            
        }
    }
}