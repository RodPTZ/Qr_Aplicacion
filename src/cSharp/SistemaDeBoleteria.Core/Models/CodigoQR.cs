using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QRCoder;

namespace SistemaDeBoleteria.Core.Models
{
    public class CodigoQR
    {
        public int IdQR { get; set; }
        public int IdEntrada { get; set; }
        public enum estadoQR { Ok, YaUsada, Expirada, FirmaInvalida, NoExiste }
        public string Codigo  { get; set; }
        public estadoQR TipoEstado  { get; set; }
        public Entrada entrada;
        public CodigoQR(int idEntrada, string codigo)
        {
            IdEntrada = idEntrada;
            Codigo = codigo;
            TipoEstado = estadoQR.NoExiste;
        }
        public CodigoQR()
        { 
            
        }
        // public void GenerarQR()
        // {
        //     using (var qrGenerator = new QRCodeGenerator())
        //     {
        //         var qrCodeData = qrGenerator.CreateQrCode(Codigo, QRCodeGenerator.ECCLevel.Q);
        //         var qrCode = new SvgQRCode(qrCodeData);
        //         var qrCodeImage = qrCode.GetGraphic(20);
        //     }
        // }


        public void Validar()
        {
            // //Para test: 
            // // TimeOnly Ahora = TimeOnly.FromDateTime(DateTime.Now.AddHours(14));

            // TimeSpan Ahora = DateTime.Now.TimeOfDay;
            // bool esHoy = entrada.FechaLiquidez.Date == DateTime.Now.Date;
            // bool dentroDelHorario = Ahora >= entrada.orden.funcion.Apertura && Ahora <= entrada.orden.funcion.Cierre;

            // if (entrada.Anulada == true)
            // {
            //     Estado = estadoQR.FirmaInvalida;
            //     return;
            // }

            // if (esHoy)
            // {
            //     if (dentroDelHorario)
            //     {
            //         if (Estado == estadoQR.Ok)
            //         {
            //             Estado = estadoQR.YaUsada;
            //         }
            //         else if (Estado != estadoQR.YaUsada)
            //         {
            //             Estado = estadoQR.Ok;
            //         }
            //     }
            //     else
            //     {
            //         Estado = estadoQR.FirmaInvalida;
            //     }
            // }
            // else if (entrada.FechaLiquidez.Date < DateTime.Now.Date)
            // {
            //     Estado = estadoQR.Expirada;
            // }
        }
    }
}