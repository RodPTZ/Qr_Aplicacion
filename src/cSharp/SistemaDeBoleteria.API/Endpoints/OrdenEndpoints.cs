using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace SistemaDeBoleteria.API.Endpoints
{
    public static class OrdenEndpoints
    {
        public static void MapOrdenEndpoints(this WebApplication app)
        {
            app.MapPost("/ordenes",
                ([FromBody] CrearOrdenDTO orden,
                 [FromServices] IOrdenService ordenService,
                 [FromServices] IValidator<CrearOrdenDTO> validator) =>
                {
                    var result = validator.Validate(orden);
                    if (!result.IsValid)
                    {
                        var errores = result.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToArray()
                            );
                        return Results.ValidationProblem(errores);
                    }

                    var ordenCreada = ordenService.Post(orden);
                    return ordenCreada is null
                        ? Results.BadRequest()
                        : Results.Created($"/ordenes/{ordenCreada.IdOrden}", ordenCreada);
                })
                .WithTags("G - Orden")
                .RequireAuthorization("EmpleadoOrganizador");

            app.MapGet("/ordenes/tipoDePago", () =>
            {
                var tiposDePago = Enum.GetValues<ETipoDePago>()
                    .ToDictionary(tp => tp.ToString(), tp => (int)tp);
                return Results.Ok(tiposDePago);
            })
            .WithTags("G - Orden")
            .RequireAuthorization("EmpleadoOrganizador");

            app.MapGet("/ordenes",
                ([FromServices] IOrdenService ordenService) =>
                {
                    var ordenes = ordenService.GetAll();
                    return !ordenes.Any() ? Results.NoContent() : Results.Ok(ordenes);
                })
                .WithTags("G - Orden")
                .RequireAuthorization("EmpleadoOrganizador");

            app.MapGet("/ordenes/{ordenID}",
                ([FromRoute] int ordenID,
                 [FromServices] IOrdenService ordenService) =>
                {
                    var orden = ordenService.Get(ordenID);
                    return orden is null ? Results.NotFound() : Results.Ok(orden);
                })
                .WithTags("G - Orden")
                .RequireAuthorization("EmpleadoOrganizador");

            app.MapPut("/ordenes/{ordenID}/pagar",
                ([FromRoute] int ordenID,
                 [FromServices] IOrdenService ordenService) =>
                {
                    ordenService.PagarOrden(ordenID);
                    return Results.Ok(new { message = "Pagado Exitosamente." });
                })
                .WithTags("G - Orden")
                .RequireAuthorization("EmpleadoOrganizador");

            app.MapPut("/ordenes/{ordenID}/cancelar",
                ([FromRoute] int ordenID,
                 [FromServices] IOrdenService ordenService) =>
                {
                    ordenService.CancelarOrden(ordenID);
                    return Results.Ok(new { message = "Cancelado Exitosamente." });
                })
                .WithTags("G - Orden")
                .RequireAuthorization("EmpleadoOrganizador");
        }
    }
}
