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
    public static class ClienteEndpoints
    {
        public static void MapClienteEndpoints(this WebApplication app)
        {
            app.MapPost("/clientes",
                ([FromBody] CrearClienteDTO cliente,
                 [FromServices] IClienteService clienteService,
                 [FromServices] IValidator<CrearClienteDTO> validator) =>
                {
                    var result = validator.Validate(cliente);
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

                    var mostrarClienteDTO = clienteService.Post(cliente);
                    return Results.Created($"/clientes/{mostrarClienteDTO.IdCliente}", mostrarClienteDTO);
                })
                .WithTags("F - Clientes")
                .RequireAuthorization("EmpleadoOrganizador");;

            app.MapGet("/clientes",
                (IClienteService clienteService) =>
                {
                    var clientes = clienteService.GetAll();
                    if (!clientes.Any())
                        return Results.NoContent();

                    return Results.Ok(clientes);
                })
                .WithTags("F - Clientes")
                .RequireAuthorization("EmpleadoOrganizador");;

            app.MapGet("/clientes/{clienteID}",
                ([FromRoute] int clienteID,
                 IClienteService clienteService) =>
                {
                    var cliente = clienteService.GetById(clienteID);
                    if (cliente is null)
                        return Results.NotFound();

                    return Results.Ok(cliente);
                })
                .WithTags("F - Clientes")
                .RequireAuthorization("EmpleadoOrganizador");

            app.MapPut("/clientes/{clienteID}",
                ([FromRoute] int clienteID,
                 [FromBody] ActualizarClienteDTO cliente,
                 [FromServices] IClienteService clienteService,
                 [FromServices] IValidator<ActualizarClienteDTO> validator) =>
                {
                    var result = validator.Validate(cliente);
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

                    var clienteActualizado = clienteService.Put(cliente, clienteID);
                    return Results.Ok(clienteActualizado);
                })
                .WithTags("F - Clientes")
                .RequireAuthorization("EmpleadoOrganizador");
        }
    }
}
