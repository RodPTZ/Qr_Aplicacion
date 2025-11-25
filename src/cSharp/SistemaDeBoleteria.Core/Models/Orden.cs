using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Enums;

namespace SistemaDeBoleteria.Core.Models
{
    public class Orden
    {
        public int IdOrden { get; set; }
        public int IdTarifa { get; set; }
        public int IdCliente { get; set; }
        public ETipoEstadoOrden Estado { get; set; }
        public ETipoDePago MedioDePago  { get; set; }
        public DateTime Emision  { get; set; }
        public DateTime Cierre  { get; set; }
        public Cliente cliente;
        public Orden(int IdTarifa,int IdCliente, ETipoEstadoOrden eTipoEstadoOrden, ETipoDePago eTipoDePago, DateTime emision, DateTime cierre)
        {
            this.IdTarifa= IdTarifa;
            this.IdCliente = IdCliente;
            Estado=eTipoEstadoOrden;
            MedioDePago = eTipoDePago;
            Emision = emision;
            Cierre = cierre;
            Emision = emision;
            Cierre = cierre;
        }
        public Orden()
        {
        }

        
       
        // public void Abonar()
        // {
        //     if (Cierre < DateTime.Now)
        //     {
        //         Console.WriteLine("No se puede abonar una orden después de su cierre.");
        //         return;
        //     }
        //     Abonado = true;
        //     sesion.entradasVendidas.Add(new Entrada(tipoEntrada, sesion.evento, this));
        // }
        // public void Cancelar()
        // {
        //     Cancelado = true;
        // }
    }
}