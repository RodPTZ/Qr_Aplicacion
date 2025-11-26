using SistemaDeBoleteria.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace SistemaDeBoleteria.API.Endpoints
{
    public static class CodigoQREndpoints
    {
        public static void MapCodigoQREndpoints(this WebApplication app)
        {
            app.MapGet("/entradas/{entradaID}/qr",
                ([FromRoute] int entradaID,
                 [FromServices] ICodigoQRService codigoQRService,
                 [FromServices] IEntradaService entradaService) =>
                {
                    var QrPng = codigoQRService.GetQRByEntradaId(entradaID);
                    return QrPng is null ? Results.NotFound() : Results.File(QrPng, "image/png");
                })
                .WithTags("I - CodigoQR")
                .RequireAuthorization("Cliente");

            app.MapGet("/qr/validar",
                ([FromQuery] int idEntrada,
                 [FromQuery] string Codigo,
                 [FromServices] ICodigoQRService codigoQRService) =>
                {
                    var estado = codigoQRService.ValidateQR(idEntrada, Codigo);
                    return Results.Ok(new { Estado = estado });
                })
                .WithTags("I - CodigoQR")
                .RequireAuthorization("Empleado");
        }
    }
}
