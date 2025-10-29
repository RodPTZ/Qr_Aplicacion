using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Repositories;
using QRCoder;
namespace SistemaDeBoleteria.Services
{
    public class CodigoQRService : ICodigoQRService
    {
        private readonly CodigoQRRepository codigoQRRepository = new CodigoQRRepository();

        public byte[]? GetQRByEntradaId(int idEntrada)
        {
            var codigoQR = codigoQRRepository.SelectById(idEntrada);

            if (codigoQR is null)
                return null;

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(codigoQR!.Codigo, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new SvgQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            byte[] qrCodeBytes = System.Text.Encoding.UTF8.GetBytes(qrCodeImage);
            return qrCodeBytes;
        }
        public string ValidateQR(int IdEntrada)
        {
            // codigoQRRepository.UpdateEstado(IdEntrada);
            return "a";
        }
    }
}