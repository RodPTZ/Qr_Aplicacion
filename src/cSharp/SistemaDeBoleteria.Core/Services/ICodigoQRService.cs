using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Services;

public interface ICodigoQRService
{
    byte[] GetQRByEntradaId(int idEntrada);
    string ValidarQR(int IdEntrada);
}
