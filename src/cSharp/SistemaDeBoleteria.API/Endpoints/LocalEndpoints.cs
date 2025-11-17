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
    public static class LocalEndpoints
    {
        public static void MapLocalEndpoints(this WebApplication app)
        {
            app.MapPost("/locales",
                ([FromBody] CrearActualizarLocalDTO local,
                 [FromServices] ILocalService localService,
                 [FromServices] IValidator<CrearActualizarLocalDTO> validator) =>
                {
                    var result = validator.Validate(local);
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

                    var localCreado = localService.Post(local);
                    return Results.Created($"/locales/{localCreado.IdLocal}", localCreado);
                })
                .WithTags("A - Locales")
                .RequireAuthorization("EmpleadoOrganizador");

            app.MapGet("/locales",
                ([FromServices] ILocalService localService) =>
                {
                    var locales = localService.GetAll();
                    return !locales.Any() ? Results.NoContent() : Results.Ok(locales);
                })
                .WithTags("A - Locales")
                .RequireAuthorization("EmpleadoOrganizador");

            app.MapGet("/locales/{localID}",
                ([FromRoute] int localID,
                 [FromServices] ILocalService localService) =>
                {
                    var local = localService.Get(localID);
                    return local is null ? Results.NotFound() : Results.Ok(local);
                })
                .WithTags("A - Locales")
                .RequireAuthorization("EmpleadoOrganizador");

            app.MapPut("/locales/{localID}",
                ([FromRoute] int localID,
                 [FromBody] CrearActualizarLocalDTO local,
                 [FromServices] ILocalService localService,
                 [FromServices] IValidator<CrearActualizarLocalDTO> validator) =>
                {
                    var result = validator.Validate(local);
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

                    var localActualizado = localService.Put(local, localID);
                    return Results.Ok(localActualizado);
                })
                .WithTags("A - Locales")
                .RequireAuthorization("EmpleadoOrganizador");

            app.MapDelete("/locales/{localID}",
                ([FromRoute] int localID,
                 [FromServices] ILocalService localService) =>
                {
                    localService.Delete(localID);
                    return Results.Ok(new { mensaje = "Local eliminado correctamente." });
                })
                .WithTags("A - Locales")
                .RequireAuthorization("EmpleadoOrganizador");
        }
    }
}
