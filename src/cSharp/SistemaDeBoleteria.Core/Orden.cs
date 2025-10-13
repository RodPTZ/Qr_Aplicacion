using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core
{
    public class Orden
    {
        public int IdOrden  { get; set; }
        public Sesion sesion;
        public int IdSesion { get; set; }
        public enum TipoEntrada { General, VIP, Plus }
        public TipoEntrada tipoEntrada  { get; set; }
        public bool Abonado  { get; set; }
        public bool Cancelado  { get; set; }
        public string MedioDePago  { get; set; }
        public DateTime Emision  { get; set; }
        public DateTime Cierre  { get; set; }
        public Cliente cliente;
        public int IdCliente { get; set; }
        public Orden(TipoEntrada tipoEntrada, Cliente cliente, string mediosDePago, Sesion sesion)
        {
            this.tipoEntrada = tipoEntrada;
            this.cliente = cliente;
            this.sesion = sesion;
            MedioDePago = mediosDePago;
            Emision = DateTime.Now;
            Cierre = Emision.AddMinutes(15);
            Abonado = false;
            Cancelado = false;
        }
        public Orden()
        {   
        }
        public void Abonar()
        {
            if (Cierre < DateTime.Now)
            {
                Console.WriteLine("No se puede abonar una orden después de su cierre.");
                return;
            }
            Abonado = true;
            sesion.entradasVendidas.Add(new Entrada(tipoEntrada, sesion.evento, this));
        }
        public void Cancelar()
        {
            Cancelado = true;
        }
    }
}