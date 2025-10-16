using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core
{
    public class Sector
    {
        public int IdSector { get; set; }
        public string TipoSector { get; set; }
        public Local local;
        public int IdLocal { get; set; }
        public Sector(string TipoSector, Local local)
        {
            this.TipoSector = TipoSector;
            this.local = local;
            local.sectores.Add(this);
        }
        public Sector()
        {
        }
    }
}