using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Repositories;

namespace SistemaDeBoleteria.Services
{
    public class EventoService : IEventoService
    {
        private readonly EventoRepository eventoRepository = new EventoRepository();
        public IEnumerable<MostrarEventoDTO> GetAll() => eventoRepository.SelectAll().Adapt<IEnumerable<MostrarEventoDTO>>();
        public MostrarEventoDTO? GetById(int IdEvento) => eventoRepository.Select(IdEvento).Adapt<MostrarEventoDTO>();
        public MostrarEventoDTO Post(CrearActualizarEventoDTO evento) => eventoRepository.Insert(evento.Adapt<Evento>()).Adapt<MostrarEventoDTO>();
        public MostrarEventoDTO Put(CrearActualizarEventoDTO evento, int IdEvento)
        {
            var eventoExiste = GetById(IdEvento);
            if (eventoExiste is null)
                return null!;
            return ;
        }
        public bool PublicarEvento(int IdEvento) =>
        public bool CancelarEvento(int IdEvento) =>
    }
}