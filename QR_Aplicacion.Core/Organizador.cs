using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QR_Aplicacion.Core
{
    public class Organizador : Usuario
    {
        public Organizador(string nombre, string apellido, int dni, string email) : base(nombre, apellido, dni, email)
        { 
            
        }
    }
}