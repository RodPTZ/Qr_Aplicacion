using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Email { get; set; }
        public string Clave { get; set; }

        public Usuario(string email, string clave)
        {
            Email = email;
            Clave = clave;
        }
        public Usuario()
        {

        }
    }
}