using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Enums;
namespace SistemaDeBoleteria.Core.Models
{
    public class Tarifa
    {
        public int IdTarifa { get; set; }
        public int IdFuncion { get; set; }
        public ETipoEntrada TipoEntrada { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public ETipoEstadoTarifa Estado { get; set; }
        public Funcion Funcion {get; set;}
        public Tarifa()
        {
        }
        
        
    }
}