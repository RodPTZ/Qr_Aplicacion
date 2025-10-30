using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Orden
    {
        public int IdOrden { get; set; }
        public int IdCliente { get; set; }
        public int IdSesion { get; set; }
        public TipoEntrada tipoEntrada  { get; set; }
        public TipoEstado Estado { get; set; }
        public TipoDePago MedioDePago  { get; set; }
        public DateTime Emision  { get; set; }
        public DateTime Cierre { get; set; }
        
        public Cliente cliente;
        public Sesion sesion;
        
        public Orden(TipoEntrada tipoEntrada, Cliente cliente, Sesion sesion)
        {
            this.tipoEntrada = tipoEntrada;
            this.cliente = cliente;
            this.sesion = sesion;
            Emision = DateTime.Now;
            Cierre = Emision.AddMinutes(15);
        }
        public Orden()
        {
        }
        public enum TipoEntrada
        {
            General,
            VIP,
            Plus
        }
        public enum TipoDePago
        {
            Efectivo,
            Transferencia,
            Debito,
            Credito
        }
        public enum TipoEstado
        {
            Abonado,
            Cancelado
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