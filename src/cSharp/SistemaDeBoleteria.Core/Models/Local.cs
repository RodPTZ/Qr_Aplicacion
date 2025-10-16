using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core
{
    public class Local
    {
        public int IdLocal { get; set; }
        public List<Sector> sectores;
        public string Ubicacion { get; set; }

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