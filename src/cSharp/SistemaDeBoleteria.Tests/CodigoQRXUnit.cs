using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SistemaDeBoleteria.Core;

namespace SistemaDeBoleteria.Tests
{
    public class CodigoQRXUnit
    {
        public Evento evento = new Evento("Concierto de Rock", Evento.TipoEvento.Musica);

        public Cliente cliente = new Cliente("Nicolas", "Gonzalez", "CABA", 12345678, "nicolas.gonzalez@gmail.com", 123456789, 4);



        // [Theory]
        // [InlineData()]
        public void RepetirValidación_CodigoQR_SoloDa_YaUsada()
        {

            DateTime ahora = DateTime.Now.AddHours(12); //12:00 AM

            Sesion sesion = new Sesion(
                25,
                DateOnly.FromDateTime(ahora),
                TimeOnly.FromDateTime(ahora.AddHours(-1)), //11:00 AM
                TimeOnly.FromDateTime(ahora.AddHours(3)), //15:00 PM
                evento
            );

            Orden orden = new Orden(Orden.TipoEntrada.General,cliente, "Tarjeta de crédito", sesion);

            orden.Abonar();

            Entrada entrada = sesion.entradasVendidas[0];

            CodigoQR codigoQR = entrada.QR;

            Assert.Equal(CodigoQR.estadoQR.NoExiste, codigoQR.Estado);

            codigoQR.Validar();
            Assert.Equal(CodigoQR.estadoQR.Ok, codigoQR.Estado);

            codigoQR.Validar();
            Assert.Equal(CodigoQR.estadoQR.YaUsada, codigoQR.Estado);

            codigoQR.Validar();
            Assert.Equal(CodigoQR.estadoQR.YaUsada, codigoQR.Estado);

        }
    }
}