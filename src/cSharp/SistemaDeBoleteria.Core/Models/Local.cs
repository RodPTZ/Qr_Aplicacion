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
        public Local(int IdLocal, string ubicacion)
        {
            this.IdLocal=IdLocal;
            Ubicacion = ubicacion;
            sectores = new List<Sector>();
        }
        public Local()
        { 
        }
    }
}