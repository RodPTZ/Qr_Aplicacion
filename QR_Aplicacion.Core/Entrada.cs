using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QR_Aplicacion.Core
{
    public class Entrada
    {
        public Cliente cliente;
        public QR qR;
        public DateTime Emision;
        public DateTime Vencimiento;
        public string Código;
        public Entrada(Cliente _cliente, QR qr,DateTime emision, DateTime vencimiento, string código)
        {
            cliente = _cliente;
            Emision = emision;
            Vencimiento = vencimiento;
            Código = código;
            qR = qr;
        }
    }
}