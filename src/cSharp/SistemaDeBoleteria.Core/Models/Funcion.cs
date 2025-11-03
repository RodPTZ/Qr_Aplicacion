using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Funcion
    {
        public int IdFuncion { get; set; }
        public int IdEvento { get; set; }
        public int IdSector { get; set; }
        public DateTime Apertura { get; set; }
        public DateTime Cierre { get; set; }
        public bool Cancelado { get; set; }
        public Sector sector;
        public Evento evento;
        public List<Tarifa> tarifas;
        public Funcion( Evento evento, Sector sector)
        {
            this.evento = evento;
            tarifas = new List<Tarifa>();
            evento.funciones.Add(this);
            Cancelado = false;
            this.sector = sector;
        }
        public Funcion()
        {
        }
    }
}