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
    public static class FuncionEndpoints
    {
        public static void MapFuncionEndpoints(this WebApplication app)
        {
            app.MapPost("/funciones",
                ([FromBody] CrearFuncionDTO funcion,
                 [FromServices] IFuncionService funcionService,
                 [FromServices] IValidator<CrearFuncionDTO> validator) =>
                {
                    var result = validator.Validate(funcion);
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

                    var funcionCreada = funcionService.Post(funcion);
                    return Results.Created($"/funciones/{funcionCreada.IdFuncion}", funcionCreada);
                })
                .WithTags("D - Funciones")
                .RequireAuthorization("Organizador");;

            app.MapGet("/funciones",
                ([FromServices] IFuncionService funcionService) =>
                {
                    var funciones = funcionService.GetAll();
                    return !funciones.Any() ? Results.NoContent() : Results.Ok(funciones);
                })
                .WithTags("D - Funciones")
                .RequireAuthorization("ClienteOrganizador");;

            app.MapGet("/funciones/{funcionID}",
                ([FromRoute] int funcionID,
                 [FromServices] IFuncionService funcionService) =>
                {
                    var funcion = funcionService.Get(funcionID);
                    return funcion is null ? Results.NotFound("No se encontró la función especificada") : Results.Ok(funcion);
                })
                .WithTags("D - Funciones")
                .RequireAuthorization("ClienteOrganizador");;

            app.MapPut("/funciones/{funcionID}",
                ([FromRoute] int funcionID,
                 [FromBody] ActualizarFuncionDTO funcion,
                 [FromServices] IFuncionService funcionService,
                 [FromServices] IValidator<ActualizarFuncionDTO> validator) =>
                {
                    var result = validator.Validate(funcion);
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

                    var funcionActualizada = funcionService.Put(funcion, funcionID);
                    return Results.Ok(funcionActualizada);
                })
                .WithTags("D - Funciones")
                .RequireAuthorization("Organizador");

            app.MapPost("/funciones/{funcionID}/cancelar",
                ([FromRoute] int funcionID,
                 [FromServices] IFuncionService funcionService) =>
                {
                    funcionService.Cancelar(funcionID);
                    return Results.Ok("Función cancelada exitosamente.");
                })
                .WithTags("D - Funciones")
                .RequireAuthorization("Organizador");
        }
    }
}
