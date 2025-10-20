using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Localidad { get; set; }
        public int DNI { get; set; }
        public string Email { get; set; }
        public int Telefono { get; set; }
        public int Edad { get; set; }
        public string Contraseña { get; set; }
        public Cliente(string nombre, string apellido, string localidad, int dni, string email, int telefono, int edad, string contraseña)
        {
            Nombre = nombre;
            Apellido = apellido;
            Localidad = localidad;
            DNI = dni;
            Email = email;
            Telefono = telefono;
            Edad = edad;
            Contraseña = contraseña;
        }
        public Cliente()
        {
        }

    }
}