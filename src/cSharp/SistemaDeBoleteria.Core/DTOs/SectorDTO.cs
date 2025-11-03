using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class CrearActualizarSectorDTO
    {
        public short Capacidad { get; set; }
    }
    public class MostrarSectorDTO
    {
        public int IdSector { get; set; }
        public int IdLocal { get; set; }
        public short Capacidad { get; set; }
    }
}