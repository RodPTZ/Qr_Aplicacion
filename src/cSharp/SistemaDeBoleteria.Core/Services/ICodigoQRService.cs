namespace SistemaDeBoleteria.Core.Services;

public interface ICodigoQRService
{
    CodigoQR? GetQRByEntradaId(int idEntrada);
    void ValidarQR(int IdEntrada);
}
