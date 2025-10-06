namespace SistemaDeBoleteria.Core
{
    public class Sesion
    {
        public int IdSesion { get; set; }
        public ushort cupos { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly Apertura { get; set; }
        public TimeOnly Cierre { get; set; }
        public List<Entrada> entradasVendidas;
        public List<Funcion> funciones;
        public Evento evento;
        public int IdEvento { get; set; }
        public Sesion(ushort cupos, DateOnly fecha, TimeOnly apertura, TimeOnly cierre, Evento evento)
        {
            this.cupos = cupos;
            Fecha = fecha;
            Apertura = apertura;
            Cierre = cierre;
            this.evento = evento;
            entradasVendidas = new List<Entrada>();
            funciones = new List<Funcion>();
            evento.sesiones.Add(this);
        }
        public Sesion()
        {
        }
    }
}