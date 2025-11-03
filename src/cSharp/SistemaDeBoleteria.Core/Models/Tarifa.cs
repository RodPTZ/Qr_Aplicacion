using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Tarifa
    {
        public int IdTarifa { get; set; }
        public int IdFuncion { get; set; }
        public TipoDeEntrada TipoEntrada { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public TipoEstado Estado { get; set; }
        public Tarifa()
        {
        }
        public enum TipoEstado
        {
            Activa,
            Inactiva,
            Agotada,
            Suspendida
        }
        public enum TipoDeEntrada
        {
            General,
            VIP,
            Plus
        }
    }
}