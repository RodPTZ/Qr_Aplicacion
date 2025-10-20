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
        public int IdSesion { get; set; }
        public List<Tarifa> tarifas;
        public TimeOnly Duración { get; set; }
        public DateTime Fecha { get; set; }
        public Evento evento;
        public bool cancelado { get; set; }
        public Sector sector;
        public Funcion(TimeOnly duracion, DateTime fecha, Evento evento, Sector sector)
        {
            Duración = duracion;
            Fecha = fecha;
            this.evento = evento;
            tarifas = new List<Tarifa>();
            evento.funciones.Add(this);
            cancelado = false;
            this.sector = sector;
        }
        public Funcion()
        {
        }
    }
}