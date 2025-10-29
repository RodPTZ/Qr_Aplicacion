using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface ICodigoQRService
{
    byte[] GetQRByEntradaId(int idEntrada);
    string ValidateQR(int IdEntrada);
}
