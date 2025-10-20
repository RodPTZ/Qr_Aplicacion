using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Tarifa
    {
        public int IdTarifa { get; set; }
        public decimal precio { get; set; }
        public short stock { get; set; }
        public string estado { get; set; }
        public Funcion funcion;
        public int IdFuncion { get; set; }
        public Tarifa(decimal precio, short stock, string estado, Funcion funcion)
        {
            this.precio = precio;
            this.stock = stock;
            this.estado = estado;
            this.funcion = funcion;
        }
        public Tarifa()
        {
        }
    }
}