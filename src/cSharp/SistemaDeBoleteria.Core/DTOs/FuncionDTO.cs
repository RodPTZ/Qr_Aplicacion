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
        public DateOnly Fecha { get; set; }
        public TimeOnly AperturaTime { get; set; }
        public TimeOnly CierreTime { get; set; }
    }
    public class ActualizarFuncionDTO
    {
        public int IdSector { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly AperturaTime { get; set; }
        public TimeOnly CierreTime { get; set; }
    }
    public class MostrarFuncionDTO
    {
        public int IdFuncion { get; set; }
        public int IdEvento { get; set; }
        public int IdSector { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly AperturaTime { get; set; }
        public TimeOnly CierreTime { get; set; }
        public DateTime Apertura { get; set; }
        public DateTime Cierre { get; set; }
        public bool Cancelado { get; set; }
    }
    
}