using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.DTOs
{
    public class CrearActualizarEventoDTO
    {
        public int IdLocal { get; set; }
        public string Nombre { get; set; }
        public Evento.TipoEvento Tipo { get; set; }
    }
    public class MostrarEventoDTO
    {
        public int IdEvento { get; set; }
        public int IdLocal { get; set; }
        public string Nombre { get; set; }
        public Evento.TipoEvento Tipo { get; set; }
        public bool Publicado { get; set; }
    }
}