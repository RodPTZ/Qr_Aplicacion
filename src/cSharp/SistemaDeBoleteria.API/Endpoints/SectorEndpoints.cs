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
    public static class SectorEndpoints
    {
        public static void MapSectorEndpoints(this WebApplication app)
        {
            app.MapPost("/locales/{localID}/sectores",
                ([FromRoute] int localID,
                 [FromBody] CrearActualizarSectorDTO sector,
                 [FromServices] ISectorService sectorService,
                 [FromServices] IValidator<CrearActualizarSectorDTO> validator) =>
                {
                    var result = validator.Validate(sector);
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

                    var nuevoSector = sectorService.Post(sector, localID);
                    return nuevoSector is null
                        ? Results.BadRequest()
                        : Results.Created($"/locales/{localID}/sectores/{nuevoSector.IdSector}", nuevoSector);
                })
                .WithTags("B - Sector")
                .RequireAuthorization("Organizador");

            app.MapGet("/locales/{localID}/sectores",
                ([FromRoute] int localID,
                 [FromServices] ISectorService sectorService) =>
                {
                    var sectores = sectorService.GetAllByLocalId(localID);
                    return !sectores.Any() ? Results.NoContent() : Results.Ok(sectores);
                })
                .WithTags("B - Sector")
                .RequireAuthorization("Organizador");

            app.MapPut("/sectores/{sectorID}",
                ([FromRoute] int sectorID,
                 [FromBody] CrearActualizarSectorDTO sector,
                 [FromServices] ISectorService sectorService,
                 [FromServices] IValidator<CrearActualizarSectorDTO> validator) =>
                {
                    var result = validator.Validate(sector);
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

                    var sectorActualizado = sectorService.Put(sector, sectorID);
                    return Results.Ok(sectorActualizado);
                })
                .WithTags("B - Sector")
                .RequireAuthorization("Organizador");

            app.MapDelete("/sectores/{sectorID}",
                ([FromRoute] int sectorID,
                 [FromServices] ISectorService sectorService) =>
                {
                    sectorService.Delete(sectorID);
                    return Results.Ok(new { message = "Sector eliminado exitosamente." });
                })
                .WithTags("B - Sector")
                .RequireAuthorization("Organizador");
        }
    }
}
