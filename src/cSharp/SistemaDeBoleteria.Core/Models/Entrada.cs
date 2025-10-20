using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Entrada
    {
        public int IdEntrada { get; set; }
        public Orden.TipoEntrada Categoria { get; set; }
        public DateTime FechaLiquidez { get; set; }
        public DateTime FechaEmisión { get; set; }
        public CodigoQR QR;
        public bool Anulada { get; set; }
        public Orden orden { get; set; }

        public Entrada(Orden.TipoEntrada categoria, Evento evento, Orden orden)
        {
            Categoria = categoria;
            FechaLiquidez = orden.sesion.Fecha.ToDateTime(orden.sesion.Apertura);
            FechaEmisión = DateTime.Now;
            QR = new CodigoQR(this.IdEntrada, $"{evento.Nombre}-{orden.IdOrden}-{FechaEmisión}-{categoria}-{orden.cliente?.DNI}");
            Anulada = false;
            this.orden = orden;
        }
    
    }
}