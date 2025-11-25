using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Enums;
namespace SistemaDeBoleteria.Core.Models
{
    public class Entrada
    {
        public int IdEntrada { get; set; }
        public int IdOrden { get; set; }
        public ETipoEntrada TipoEntrada { get; set; }
        public DateTime Emision { get; set; }
        public DateTime Liquidez { get; set; }
        public bool Anulado { get; set; }
        public CodigoQR QR;
        public Entrada()
        {
            
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