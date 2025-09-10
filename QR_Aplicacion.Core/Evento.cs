using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QR_Aplicacion.Core
{
    public class Evento
    {
        public string Ubicación;
        public List<Entrada> entradas;
        public DateTime Fecha;
        public Evento(string ubicación, DateTime fecha)
        {
            Ubicación = ubicación;
            Fecha = fecha;
            entradas = new List<Entrada>();
        }
    }
}