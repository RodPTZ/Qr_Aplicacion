using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QR_Aplicacion.Core
{
    public abstract class Usuario
    {
        public string Nombre;
        public string Apellido;
        public int DNI;
        public string Email;

        public Usuario(string nombre, string apellido, int dni, string email)
        {
            Nombre = nombre;
            Apellido = apellido;
            DNI = dni;
            Email = email;
        }
    }
}