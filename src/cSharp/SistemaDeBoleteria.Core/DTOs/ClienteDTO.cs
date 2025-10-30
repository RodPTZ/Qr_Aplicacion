using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class CrearClienteDTO : ActualizarClienteDTO
    {
        public int DNI { get; set; }
        public int Edad { get; set; }
        public string Contraseña { get; set; }
    }
    public class ActualizarClienteDTO
    {
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Localidad { get; set; }
        public int Telefono { get; set; }
        public string Email { get; set; }
    }
    public class MostrarClienteDTO
    {
        public int IdCliente { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Localidad { get; set; }
        public int Telefono { get; set; }
        public int DNI { get; set; }
        public int Edad { get; set; }
        public string Usuario { get; set; }
        public string Email { get; set; }
    }
}