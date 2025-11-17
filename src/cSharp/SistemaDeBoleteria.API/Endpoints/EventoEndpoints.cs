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
    public static class EventoEndpoints
    {
        public static void MapEventoEndpoints(this WebApplication app)
        {
            app.MapPost("/eventos",
                ([FromBody] CrearActualizarEventoDTO evento,
                 [FromServices] IEventoService eventoService,
                 [FromServices] IValidator<CrearActualizarEventoDTO> validator) =>
                {
                    var result = validator.Validate(evento);
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

                    var eventoCreado = eventoService.Post(evento);
                    return Results.Created($"/eventos/{eventoCreado.IdEvento}", eventoCreado);
                })
                .WithTags("C - Eventos");

            app.MapGet("/eventos/tipos",
                () =>
                {
                    var tiposDeEvento = Enum.GetValues<ETipoEvento>()
                        .ToDictionary(t => t.ToString(), t => (int)t);

                    return Results.Ok(tiposDeEvento);
                })
                .WithTags("C - Eventos");

            app.MapGet("/eventos",
                ([FromServices] IEventoService eventoService) =>
                {
                    var eventos = eventoService.GetAll();
                    return !eventos.Any() ? Results.NoContent() : Results.Ok(eventos);
                })
                .WithTags("C - Eventos");

            app.MapGet("/eventos/{eventoID}",
                ([FromRoute] int eventoID,
                 [FromServices] IEventoService eventoService) =>
                {
                    var evento = eventoService.GetById(eventoID);
                    return evento is null ? Results.NotFound() : Results.Ok(evento);
                })
                .WithTags("C - Eventos");

            app.MapPut("/eventos/{eventoID}",
                ([FromRoute] int eventoID,
                 [FromBody] CrearActualizarEventoDTO evento,
                 [FromServices] IEventoService eventoService,
                 [FromServices] IValidator<CrearActualizarEventoDTO> validator) =>
                {
                    var result = validator.Validate(evento);
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

                    var eventoActualizado = eventoService.Put(evento, eventoID);
                    return Results.Ok(eventoActualizado);
                })
                .WithTags("C - Eventos");

            app.MapPost("/eventos/{eventoID}/publicar",
                ([FromRoute] int eventoID,
                 [FromServices] IEventoService eventoService) =>
                {
                    eventoService.PublicarEvento(eventoID);
                    return Results.Ok("Evento publicado exitosamente.");
                })
                .WithTags("C - Eventos");

            app.MapPost("/eventos/{eventoID}/cancelar",
                ([FromRoute] int eventoID,
                 [FromServices] IEventoService eventoService) =>
                {
                    eventoService.CancelarEvento(eventoID);
                    return Results.Ok("Evento cancelado exitosamente");
                })
                .WithTags("C - Eventos");
        }
    }
}
