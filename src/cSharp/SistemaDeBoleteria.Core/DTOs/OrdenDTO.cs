using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Enums;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class CrearOrdenDTO
    {
        public int IdTarifa { get; set; }
        public int IdCliente { get; set; }
        public ETipoDePago MedioDePago { get; set; }
    }
    public class MostrarOrdenDTO
    {
        public int IdOrden { get; set; }
        public int IdTarifa { get; set; }
        public int IdCliente { get; set; }
        public string Estado { get; set; }
        public string MedioDePago { get; set; }
        public DateTime Emision { get; set; }
        public DateTime Cierre { get; set; }
    }
    
}