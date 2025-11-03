using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Localidad { get; set; }
        public int DNI { get; set; }
        public string Telefono { get; set; }
        public byte Edad { get; set; }
        public Cliente(string nombre, string apellido, string localidad, int dni, string telefono, byte edad)
        {
            Nombre = nombre;
            Apellido = apellido;
            Localidad = localidad;
            DNI = dni;
            Telefono = telefono;
            Edad = edad;
        }
        public Cliente()
        {
        }

    }
}