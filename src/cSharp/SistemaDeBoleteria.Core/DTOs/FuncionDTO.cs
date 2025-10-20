using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class CrearFuncionDTO
    {
        public int IdEvento { get; set; }
        public int IdSector { get; set; }
        public int IdSesion { get; set; }
        public TimeOnly Duracion { get; set; }
        public DateTime Fecha { get; set; }
    }
    public class MostrarFuncionDTO
    {
        public int IdFuncion { get; set; }
        public int IdEvento { get; set; }
        public int IdSector { get; set; }
        public int IdSesion { get; set; }
        public TimeOnly Duracion { get; set; }
        public DateTime Fecha { get; set; }
        public bool Cancelado { get; set; }
    }
    public class ActualizarFuncionDTO
    {
        public int IdSector { get; set; }
        public int IdSesion { get; set; }
        public TimeOnly Duracion { get; set; }
        public DateTime Fecha { get; set; }
    }
}