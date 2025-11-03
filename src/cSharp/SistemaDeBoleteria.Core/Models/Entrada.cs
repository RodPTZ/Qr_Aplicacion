using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Entrada
    {
        public int IdEntrada { get; set; }
        public int IdOrden { get; set; }
        public Tarifa.TipoDeEntrada TipoEntrada { get; set; }
        public DateTime Emision { get; set; }
        public DateTime Liquidez { get; set; }
        public TipoEstado Estado { get; set; }
        public CodigoQR QR;
        public Entrada()
        {
            
        }
        public enum TipoEstado
        {
            Anulado,
            Pagado,
            Pendiente
        }
        // public Entrada(Orden.TipoEntrada categoria, Evento evento, Orden orden)
        // {
        //     Categoria = categoria;
        //     FechaLiquidez = orden.funcion.Fecha.ToDateTime(orden.funcion.Apertura);
        //     FechaEmisión = DateTime.Now;
        //     QR = new CodigoQR(this.IdEntrada, $"{evento.Nombre}-{orden.IdOrden}-{FechaEmisión}-{categoria}-{orden.cliente?.DNI}");
        //     Anulada = false;
        //     this.orden = orden;
        // }
    
    }
}