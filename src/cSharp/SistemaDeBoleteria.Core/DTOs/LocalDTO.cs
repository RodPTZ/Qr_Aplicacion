using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class CrearActualizarLocalDTO
    {
        public string Nombre { get; set; }
        public string Ubicacion { get; set; }
    }
    public class MostrarLocalDTO
    {
        public int IdLocal { get; set; }
        public string Nombre { get; set; }
        public string Ubicacion { get; set; }
    }
}