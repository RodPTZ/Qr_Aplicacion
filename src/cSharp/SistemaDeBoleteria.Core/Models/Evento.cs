using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Enums;

namespace SistemaDeBoleteria.Core.Models
{
    public class Evento
    {
        public int IdEvento { get; set; }
        public int IdLocal { get; set; }
        public string Nombre { get; set; }
        public ETipoEvento Tipo { get; set; }
        public ETipoEstadoEvento Estado { get; set; }
        public List<Funcion> funciones;

        public Evento(string nombre, ETipoEvento tipoEvento)
        {
            Nombre = nombre;
            Tipo = tipoEvento;
            funciones = new List<Funcion>();
        }
        public Evento()
        {
        }
        

        
        // public bool Publicar()
        // {
        //     if (funciones == null || funciones.Count == 0)
        //     {
        //         return false;
        //     }
        //     publicado = true;
        //     return true;
        // }
        // public bool Cancelar()
        // {
        //     publicado = false;
        //     return true;
        // }
        // public void AgregarFuncion(Funcion funcion, Sesion sesion)
        // {
        //     sesion.funciones.Add(funcion);
        //     funciones.Add(funcion);
        // }
    }
}