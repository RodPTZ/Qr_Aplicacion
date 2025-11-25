using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.Exceptions;
using Mapster;
using MySqlConnector;
using System.Data.Common;


namespace SistemaDeBoleteria.Services
{
    public class EntradaService : IEntradaService
    {
        private readonly IEntradaRepository entradaRepository;
        private readonly ICodigoQRRepository codigoQRRepository;
        public EntradaService(IEntradaRepository entradaRepository, ICodigoQRRepository codigoQRRepository)
        {
            this.entradaRepository = entradaRepository;
            this.codigoQRRepository = codigoQRRepository;
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
            if(entrada.Anulado is true)
                throw new BusinessException("No se puede anular una entrada que se encuentra anulada.");
            
            //deberia ir en trycatch
            if(!entradaRepository.UpdAnular(idEntrada))
                throw new DataBaseException("No se pudo anular la entrada.");
            if(!codigoQRRepository.UpdAYaUsada(idEntrada))
                throw new DataBaseException("No se pudo marcar el QR como ya usada, tras la cancelación de la entrada.");
            return true;
            //
            // try
            // {
            //     return entradaRepository.UpdateEstado(idEntrada);
            // }
            // catch (MySqlException ex)
            // {
            //     throw new DataBaseException(ex.Message);
            // }
            
        }
    }
}