using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core
{
    public class Evento
    {
        public int IdEvento { get; set; }
        public int IdLocal { get; set; }
        public string Nombre { get; set; }
        public enum TipoEvento {Concierto,Convencion,Opera,Teatro,Deportes,Boliches,Musica}
        public TipoEvento Tipo { get; set; }   
        public bool publicado { get; set; }
        public List<Sesion> sesiones;
        public List<Funcion> funciones;

        public Evento(string nombre, TipoEvento tipoEvento)
        {
            Nombre = nombre;
            Tipo = tipoEvento;
            sesiones = new List<Sesion>();
            funciones = new List<Funcion>();
            publicado = false;
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