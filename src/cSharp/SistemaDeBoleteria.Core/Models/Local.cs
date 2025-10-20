using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Local
    {
        public int IdLocal { get; set; }
        public string Nombre { get; set; }
        public string Ubicacion { get; set; }
        public List<Sector> sectores;
        public Local(string ubicacion)
        {
            Ubicacion = ubicacion;
            sectores = new List<Sector>();
        }
        public Local()
        { 
        }
    }
}