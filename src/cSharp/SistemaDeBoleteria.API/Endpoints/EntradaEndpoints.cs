using SistemaDeBoleteria.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace SistemaDeBoleteria.API.Endpoints
{
    public static class EntradaEndpoints
    {
        public static void MapEntradaEndpoints(this WebApplication app)
        {
            app.MapGet("/entradas",
                ([FromServices] IEntradaService entradaService) =>
                {
                    var entradas = entradaService.GetAll();
                    return !entradas.Any() ? Results.NoContent() : Results.Ok(entradas);
                })
                .WithTags("H - Entradas")
                .RequireAuthorization("Empleado");

            app.MapGet("/entradas/{entradaID}",
                ([FromRoute] int entradaID,
                 [FromServices] IEntradaService entradaService) =>
                {
                    var entrada = entradaService.GetById(entradaID);
                    return entrada is null ? Results.NotFound() : Results.Ok(entrada);
                })
                .WithTags("H - Entradas")
                .RequireAuthorization("Empleado");

            app.MapPost("/entradas/{entradaID}/anular",
                ([FromRoute] int entradaID,
                 [FromServices] IEntradaService entradaService) =>
                {
                    entradaService.AnularEntrada(entradaID);
                    return Results.Ok(new { message = "Cancelación de entrada exitosa." });
                })
                .WithTags("H - Entradas")
                .RequireAuthorization("Cliente");
        }
    }
}
