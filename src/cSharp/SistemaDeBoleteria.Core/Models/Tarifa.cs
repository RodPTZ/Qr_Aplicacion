using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Tarifa
    {
        public int IdTarifa { get; set; }
        public enum TipoEntrada { General, VIP, PLUS}
        public decimal precio { get; set; }
        public short stock { get; set; }
        public enum TipoEstado { Activa, Inactiva, Agotada, Suspendida}
        public TipoEstado estado { get; set; }
        public Funcion funcion;
        public int IdFuncion { get; set; }
        public Tarifa(decimal precio, short stock, Funcion funcion)
        {
            this.precio = precio;
            this.stock = stock;
            estado = TipoEstado.Inactiva;
            this.funcion = funcion;
        }
        public Tarifa()
        {
        }
    }
}