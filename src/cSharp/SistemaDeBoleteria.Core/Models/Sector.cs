using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Sector
    {
        public int IdSector { get; set; }
        public int IdLocal { get; set; }
        public short Capacidad { get; set; }
        public Local local;

        public Sector(short capacidad, Local local)
        {
            Capacidad= capacidad;
            this.local = local;
            local.sectores.Add(this);
        }
        public Sector()
        {
        }
    }
}