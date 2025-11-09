using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Repositories;
using QRCoder;
using SistemaDeBoleteria.Core.Models;
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
            var url = $"http://192.168.1.63:5027/qr/validar?idEntrada={codigoQR.IdEntrada}&codigoQR={codigoQR.Codigo}";
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
        public string ValidateQR(int IdEntrada, string codigo)
        {
            var Exists = codigoQRRepository.Exists(codigo);
            if (!Exists)
            {
                return codigoQRRepository
                                .UpdateEstado(IdEntrada, CodigoQR.estadoQR.NoExiste)
                                .ToString()!;
            }

            var (DataEntrada, DataFuncion, DataQR) = codigoQRRepository.SelectData(IdEntrada);
            if (DataEntrada == null || DataFuncion == null || DataQR == null)
            {
                return codigoQRRepository
                        .UpdateEstado(IdEntrada, CodigoQR.estadoQR.FirmaInvalida)
                        .ToString()!;
            }
            TimeSpan ahora = DateTime.Now.TimeOfDay;

            bool esHoy = DataEntrada.Liquidez.Date == DateTime.Now.Date;
            bool dentroDelHorario = ahora >= DataFuncion.Apertura.TimeOfDay && ahora <= DataFuncion.Cierre.TimeOfDay;

            if (DataEntrada.Estado == Entrada.TipoEstado.Anulado)
            {
                return codigoQRRepository.UpdateEstado(IdEntrada, CodigoQR.estadoQR.FirmaInvalida).ToString()!;

            }

            if (esHoy)
            {
                if (dentroDelHorario)
                {
                    if (DataQR.TipoEstado == CodigoQR.estadoQR.Ok)
                    {
                        return codigoQRRepository
                                .UpdateEstado(IdEntrada, CodigoQR.estadoQR.YaUsada)
                                .ToString()!;
                    }
                    else if (DataQR.TipoEstado != CodigoQR.estadoQR.YaUsada)
                    {
                        return codigoQRRepository
                                .UpdateEstado(IdEntrada, CodigoQR.estadoQR.Ok)
                                .ToString()!;
                    }
                }
                else
                {
                    return codigoQRRepository
                                .UpdateEstado(IdEntrada, CodigoQR.estadoQR.FirmaInvalida)
                                .ToString()!;
                }
            }
            else if (DataEntrada.Liquidez.Date > DateTime.Now.Date)
            {
                return codigoQRRepository
                            .UpdateEstado(IdEntrada, CodigoQR.estadoQR.Expirada)
                            .ToString()!;
            }
            else if (DataFuncion.Apertura.Date > DateTime.Now.Date)
            {
                return codigoQRRepository
                                .UpdateEstado(IdEntrada, CodigoQR.estadoQR.FirmaInvalida)
                                .ToString()!;
            }
            return codigoQRRepository
                                .UpdateEstado(IdEntrada, CodigoQR.estadoQR.FirmaInvalida)
                                .ToString()!;
        }
    }
}