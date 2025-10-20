using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class CrearOrdenDTO
    {
        public int IdCliente { get; set; }
        public int IdSesion { get; set; }
        public Orden.TipoEntrada tipoEntrada { get; set; }
        public Orden.TipoDePago MedioDePago { get; set; }
    }
    public class MostrarOrdenDTO
    {
        public int IdOrden { get; set; }
        public int IdCliente { get; set; }
        public int IdSesion { get; set; }
        public Orden.TipoEntrada tipoEntrada { get; set; }
        public bool Abonado { get; set; }
        public bool Cancelado { get; set; }
        public string MedioDePago { get; set; }
        public DateTime Emision { get; set; }
        public DateTime Cierre { get; set; }
    }
    
}