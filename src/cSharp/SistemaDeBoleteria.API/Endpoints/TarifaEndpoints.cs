using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace SistemaDeBoleteria.API.Endpoints
{
    public static class TarifaEndpoints
    {
        public static void MapTarifaEndpoints(this WebApplication app)
        {
            app.MapPost("/tarifas",
                ([FromBody] CrearTarifaDTO tarifa,
                 [FromServices] ITarifaService tarifaService,
                 [FromServices] IValidator<CrearTarifaDTO> validator) =>
                {
                    var result = validator.Validate(tarifa);
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

                    var mostrarTarifa = tarifaService.Post(tarifa);
                    return Results.Created($"/tarifas/{mostrarTarifa.IdTarifa}", mostrarTarifa);
                })
                .WithTags("E - Tarifas")
                .RequireAuthorization("EmpleadoOrganizador");;

            app.MapGet("/funciones/{funcionID}/tarifas",
                ([FromRoute] int funcionID,
                 [FromServices] ITarifaService tarifaService) =>
                {
                    var tarifas = tarifaService.GetAllByFuncionId(funcionID);
                    return !tarifas.Any() ? Results.NoContent() : Results.Ok(tarifas);
                })
                .WithTags("E - Tarifas")
                .RequireAuthorization("EmpleadoOrganizador");;

            app.MapPut("/tarifas/{tarifaID}",
                ([FromRoute] int tarifaID,
                 [FromBody] ActualizarTarifaDTO tarifa,
                 [FromServices] ITarifaService tarifaService,
                 [FromServices] IValidator<ActualizarTarifaDTO> validator) =>
                {
                    var result = validator.Validate(tarifa);
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

                    var mostrarTarifa = tarifaService.Put(tarifa, tarifaID);
                    return Results.Ok(mostrarTarifa);
                })
                .WithTags("E - Tarifas")
                .RequireAuthorization("EmpleadoOrganizador");;

            app.MapGet("/tarifas/{tarifaID}",
                ([FromRoute] int tarifaID,
                 [FromServices] ITarifaService tarifaService) =>
                {
                    var tarifa = tarifaService.Get(tarifaID);
                    return tarifa is null ? Results.NotFound() : Results.Ok(tarifa);
                })
                .WithTags("E - Tarifas")
                .RequireAuthorization("EmpleadoOrganizador");;
        }
    }
}
