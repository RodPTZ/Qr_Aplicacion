using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class CrearFuncionDTO : ActualizarFuncionDTO
    {
        public int IdEvento { get; set; }
    }
    public class ActualizarFuncionDTO
    {
        public int IdSector { get; set; }
        public DateTime Apertura { get; set; }
        public DateTime Cierre { get; set; }
    }
    public class MostrarFuncionDTO
    {
        public int IdFuncion { get; set; }
        public int IdEvento { get; set; }
        public int IdSector { get; set; }
        public DateTime Apertura { get; set; }
        public DateTime Cierre { get; set; }
        public bool Cancelado { get; set; }
    }
    
}