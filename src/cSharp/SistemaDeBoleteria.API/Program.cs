using Microsoft.AspNetCore.Mvc;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Validations;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Services;
using FluentValidation;
using Mapster;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Servicios

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICodigoQRService, CodigoQRService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<ILocalService, LocalService>();
builder.Services.AddScoped<IOrdenService, OrdenService>();
builder.Services.AddScoped<ISectorService, SectorService>();
builder.Services.AddScoped<ITarifaService, TarifaService>();

#endregion

#region Validaciones

builder.Services.AddTransient<IValidator<CrearClienteDTO>, ClienteValidator>();
builder.Services.AddTransient<IValidator<ActualizarClienteDTO>, ActualizarClienteDTOValidator>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Cliente

app.MapPost("/clientes", ([FromBody] CrearClienteDTO cliente, [FromServices] IClienteService clienteService, [FromServices] IValidator<CrearClienteDTO> validator) =>
{
    // var validator = new ClienteValidator();
    var result = validator.Validate(cliente);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,

            g => g.Select(e => e.ErrorMessage).ToArray()
            );
        return Results.ValidationProblem(listaErrores);
    }

    var mostrarClienteDTO = clienteService.Post(cliente);
    return Results.Created($"/clientes/{mostrarClienteDTO.IdCliente}", mostrarClienteDTO);
}).WithTags("Clientes");

app.MapGet("/clientes", (IClienteService clienteService) =>
{
    var _clientes = clienteService.GetAll();
    if (!_clientes.Any())
        return Results.NoContent();

    return Results.Ok(_clientes);
}).WithTags("Clientes");

app.MapGet("/clientes/{clienteID}", ([FromRoute] int clienteID, IClienteService clienteService) =>
{
    var cliente = clienteService.GetById(clienteID);
    if (cliente is null)
        return Results.NotFound();
        
    return Results.Ok(cliente);
    // return clienteService.GetById(clienteID) is null ? Results.NotFound() : Results.Ok(clienteService.GetById(clienteID)); 

}).WithTags("Clientes");

app.MapPut("/clientes/{clienteID}", ([FromRoute] int clienteID, [FromBody] ActualizarClienteDTO cliente, [FromServices]IClienteService clienteService, [FromServices]IValidator<ActualizarClienteDTO> validator) =>
{
    var result = validator.Validate(cliente);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,

            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }

    var clienteActualizado = clienteService.Put(cliente, clienteID);
    return clienteActualizado is null ? Results.NotFound() : Results.Ok(clienteActualizado);
}).WithTags("Clientes");
#endregion

#region CodigoQR

app.MapGet("/entradas/{entradaID}/qr", ([FromRoute] int entradaID, [FromServices] ICodigoQRService codigoQRService, [FromServices] IEntradaService entradaService) =>
{
    var Qr = codigoQRService.GetQRByEntradaId(entradaID);
    return Qr is null ? Results.NotFound() : Results.File(Qr, "image");
}).WithTags("CodigoQR");

app.MapPost("/qr/validar", ([FromBody] int IdEntrada, [FromServices] IEntradaService entradaService, [FromServices] ICodigoQRService codigoQRService) =>
{
    var estado = codigoQRService.ValidateQR(IdEntrada);
    return Results.Ok(estado);

}).WithTags("CodigoQR");

#endregion

#region  Entrada

app.MapGet("/entradas", ([FromServices] IEntradaService entradaService) =>
{
    var entradas = entradaService.GetAll();
    return !entradas.Any() ?  Results.NoContent() : Results.Ok(entradas);
}).WithTags("Entradas");

app.MapGet("/entradas/{entradaID}", ([FromRoute] int entradaID, [FromServices] IEntradaService entradaService) =>
{
    var entrada = entradaService.GetById(entradaID);
    return entrada is null ? Results.NotFound() : Results.Ok(entrada);
}).WithTags("Entradas");

app.MapPost("/entradas/{entradaID}/anular", ([FromRoute] int entradaID, [FromServices] IEntradaService entradaService) =>
{
    bool funciono = entradaService.AnularEntrada(entradaID);
    return funciono is false ? Results.NotFound(new { message = "No funciona" }) : Results.Ok(new { message = "Si funciona" });
}).WithTags("Entradas");

#endregion

#region  Evento

app.MapPost("/eventos", ([FromBody] CrearActualizarEventoDTO evento, [FromServices] IEventoService eventoService) =>
{
    // Validaciones
    var validators = new EventoValidator();
    var result = validators.Validate(evento);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }
    // Insertar
    var eventoCreado = eventoService.InsertEvento(evento);
    // Retornar
    return eventoService.GetEventoById(eventoCreado.IdEvento) is null ? Results.BadRequest() : Results.Created($"/eventos/{eventoCreado.IdEvento}", eventoCreado);
}).WithTags("Eventos");

app.MapGet("/eventos", ([FromServices] IEventoService eventoService) =>
{
    // Obtener eventos
    var eventos = eventoService.GetEventos();
    if (!eventos.Any())
        return Results.NoContent();
    // Retornar eventos
    return Results.Ok(eventos);
}).WithTags("Eventos");

app.MapGet("/eventos/{eventoID}", ([FromRoute] int eventoID, [FromServices] IEventoService eventoService) =>
{
    // Obtener evento por ID
    var evento = eventoService.GetEventoById(eventoID);
    // Retornar evento
    return evento is not null ? Results.Ok(evento) : Results.NotFound();
}).WithTags("Eventos");

app.MapPut("/eventos/{eventoID}", ([FromRoute] int eventoID, [FromBody] CrearActualizarEventoDTO evento, [FromServices] IEventoService eventoService) =>
{
    // Verificar existencia
    var _evento = eventoService.GetEventoById(eventoID);
    if (_evento is null)
        return Results.NotFound();
    // Validaciones
    var validators = new EventoValidator();
    var result = validators.Validate(evento);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }
    // Actualizar
    var eventoActualizado = eventoService.UpdateEvento(evento, eventoID);
    return Results.Ok(eventoActualizado);
}).WithTags("Eventos");

app.MapPost("/eventos/{eventoID}/publicar", ([FromRoute] int eventoID, [FromServices] IEventoService eventoService) =>
{
    // Verificar existencia
    var evento = eventoService.GetEventoById(eventoID);
    if (evento is null)
        return Results.NotFound();

    // Publicar
    if (eventoService.PublicarEvento(eventoID)) // falta verificar la salida.
        return Results.Ok(evento);

    // Retornar. No se pudo publicar
    return Results.BadRequest();
}).WithTags("Eventos");

app.MapPost("/eventos/{eventoID}/cancelar", ([FromRoute] int eventoID, [FromServices] IEventoService eventoService) =>
{
    // Verificar existencia
    var evento = eventoService.GetEventoById(eventoID);
    if (evento is null)
        return Results.NotFound();
    // Cancelar
    if (eventoService.CancelarEvento(eventoID)) // falta verificar la salida.
        return Results.Ok(evento);

    // Retornar. No se pudo cancelar
    return Results.BadRequest();
}).WithTags("Eventos");
#endregion

#region Funcion

app.MapPost("/funciones", ([FromBody] CrearFuncionDTO funcion, [FromServices] IFuncionService funcionService) =>
{
    // Validaciones
    var validators = new FuncionValidator();
    var result = validators.Validate(funcion);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }

    // Insertar
    var funcionCreada = funcionService.InsertFuncion(funcion);

    // Retornar 
    return funcionService.GetFuncionById(funcionCreada.IdFuncion) is null ? Results.BadRequest() : Results.Created($"/funciones/{funcionCreada.IdFuncion}", funcion);
}).WithTags("Funciones");

app.MapGet("/funciones", ([FromServices] IFuncionService funcionService) =>
{
    // Obtener funciones
    var funciones = funcionService.GetFunciones();
    if (!funciones.Any())
        return Results.NoContent();

    // Retornar funciones
    return Results.Ok(funciones);
}).WithTags("Funciones");

app.MapGet("/funciones/{funcionID}", ([FromRoute] int funcionID, [FromServices] IFuncionService funcionService) =>
{
    // Verificar existencia
    var funcion = funcionService.GetFuncionById(funcionID);
    // Retornar funcion
    return funcion is not null ? Results.Ok(funcion) : Results.NotFound();
}).WithTags("Funciones");

app.MapPut("/funciones/{funcionID}", ([FromRoute] int funcionID, [FromBody] ActualizarFuncionDTO funcion, [FromServices] IFuncionService funcionService) =>
{
    // Verificar existencia
    var _funcion = funcionService.GetFuncionById(funcionID);
    if (_funcion is null)
        return Results.NotFound();

    // Validaciones
    var validators = new ActualizarFuncionValidator();
    var result = validators.Validate(funcion);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }
    // Actualizar
    var funcionActualizada = funcionService.UpdateFuncion(funcion, funcionID);
    // Retornar
    return Results.Ok(funcionActualizada);
}).WithTags("Funciones");

app.MapPost("/funciones/{funcionID}/cancelar", ([FromRoute] int funcionID, [FromServices] IFuncionService funcionService) =>
{
    // Verificar existencia
    var funcion = funcionService.GetFuncionById(funcionID);
    if (funcion is null)
        return Results.NotFound();

    // Cancelar
    funcionService.CancelarFuncion(funcionID);

    // Retornar
    return Results.Ok(funcion);
}).WithTags("Funciones");
#endregion

#region Local

app.MapPost("/locales", ([FromBody] CrearActualizarLocalDTO local, [FromServices] ILocalService localeService) =>
{
    //Validaciones
    var validators = new LocalValidator();
    var result = validators.Validate(local);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }

    //Insertar
    var localCreado = localeService.InsertLocal(local);

    //Retornar
    return localeService.GetLocalById(localCreado.IdLocal) is null ? Results.BadRequest() : Results.Created($"/locales/{localCreado.IdLocal}", local);
}).WithTags("Locales");

app.MapGet("/locales", ([FromServices] ILocalService localService) =>
{
    // Obtener locales
    var locales = localService.GetLocales();
    if (!locales.Any())
        return Results.NoContent();

    // Retornar locales
    return Results.Ok(locales);
}).WithTags("Locales");

app.MapGet("/locales/{localID}", ([FromRoute] int localID, [FromServices] ILocalService localeService) =>
{
    // Obtener local por ID
    var local = localeService.GetLocalById(localID);
    // Retornar local
    return local is not null ? Results.Ok(local) : Results.NotFound();
}).WithTags("Locales");

app.MapPut("/locales/{localID}", ([FromRoute] int localID, [FromBody] CrearActualizarLocalDTO local, [FromServices] ILocalService localeService) =>
{
    // Verificar existencia
    var _local = localeService.GetLocalById(localID);
    if (_local is null)
        return Results.NotFound();

    // Validaciones
    var validators = new LocalValidator();
    var result = validators.Validate(local);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }
    // Actualizar
    var localActualizado =localeService.UpdateLocal(local, localID);
    // Retornar
    return Results.Ok(localActualizado);
}).WithTags("Locales");

app.MapDelete("/locales/{localID}", ([FromRoute] int localID, [FromServices] ILocalService localeService) =>
{
    // Verificar existencia
    var local = localeService.GetLocalById(localID);
    if (local is null)
        return Results.NotFound();
    // Eliminar
    bool fueEliminado = localeService.DeleteLocal(localID);
    if (fueEliminado)
        return Results.Ok(new { mensaje = "Local eliminado correctamente." });

    // Retorno de error
    return Results.BadRequest();
}).WithTags("Locales");
#endregion

#region Login

app.MapPost("/register", ([FromBody] Usuario usuario, [FromServices] ILoginService loginService) =>
{
    loginService.Register(usuario);
    return Results.Created($"/register/{usuario.Email}", usuario);
}).WithTags("Login");
app.MapPost("/login", () =>
{

}).WithTags("Login");
app.MapPost("/refresh", () =>
{

}).WithTags("Login");
app.MapPost("/logout", () =>
{

}).WithTags("Login");
app.MapGet("/me", () =>
{

}).WithTags("Login");
app.MapGet("/roles", () =>
{
    string[] roles = {
        "Administrador",
        "Organizador",
        "Cliente",
        "Control de Acceso"
    };
    return Results.Ok(roles);
}).WithTags("Login");
app.MapPost("/usuarios/{usuarioID}/roles", ([FromRoute] int usuarioID, [FromServices] ILoginService loginService) =>
{
    return Results.Ok();
}).WithTags("Login");
#endregion

#region Orden

app.MapPost("/ordenes", ([FromBody] CrearOrdenDTO orden, [FromServices] IOrdenService ordenService) =>
{
    // Validaciones
    var validators = new OrdenValidator();
    var result = validators.Validate(orden);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }

    // Insertar
    var ordenCreada = ordenService.InsertOrden(orden);

    // Retornar 
    return ordenService.GetOrdenById(ordenCreada.IdOrden) is null ? Results.BadRequest() : Results.Created($"/ordenes/{ordenCreada.IdOrden}", ordenCreada);
}).WithTags("Orden");

app.MapGet("/ordenes", ([FromServices] IOrdenService ordenService) =>
{
    // Validaciones
    var ordenes = ordenService.GetOrdenes();
    if (!ordenes.Any())
        return Results.NoContent();

    // Retornar
    return Results.Ok(ordenes);
}).WithTags("Orden");

app.MapGet("/ordenes/{ordenID}", ([FromRoute] int ordenID, [FromServices] IOrdenService ordenService) =>
{
    // Obtener
    var orden = ordenService.GetOrdenById(ordenID);

    // Retornar
    return orden is not null ? Results.Ok(orden) : Results.NotFound();
}).WithTags("Orden");

app.MapPost("/ordenes/{ordenID}/pagar", ([FromRoute] int ordenID, [FromServices] IOrdenService ordenService) =>
{
    var orden = ordenService.GetOrdenById(ordenID);
    if (orden is null)
        return Results.NotFound();

    ordenService.PagarOrden(ordenID);
    return Results.Ok(orden);
}).WithTags("Orden");

app.MapPost("/ordenes/{ordenID}/cancelar", ([FromRoute] int ordenID, [FromServices] IOrdenService ordenService) =>
{
    // Validaciones
    var orden = ordenService.GetOrdenById(ordenID);
    if (orden is null)
        return Results.NotFound();

    // Cancelar
    bool fueCancelado = ordenService.CancelarOrden(ordenID);
    // Retornar
    return fueCancelado is true ? Results.Ok(new { message="Cancelado Exitosamente." }) : Results.BadRequest();
}).WithTags("Orden");

#endregion

#region Sector

// 1. Crear un sector de un local
app.MapPost("/locales/{localID}/sectores", ([FromRoute] int localID, [FromBody] CrearActualizarSectorDTO sector, [FromServices] ISectorService sectorService) =>
{
    // Valdiaciones
    var validators = new SectorValidator();
    var result = validators.Validate(sector);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }

    // Insertar
    var mostrarSector = sectorService.InsertSector(sector, localID);
    
    // Retornar
    return mostrarSector is null ? Results.BadRequest() : Results.Created($"/locales/{localID}/sectores/{mostrarSector.IdSector}", mostrarSector);
});

// 2. Traer sectores de un local
app.MapGet("/locales/{localID}/sectores", ([FromRoute] int localID, [FromServices] ISectorService sectorService) =>
{
    var sectores = sectorService.GetSectoresByLocalId(localID);
    if (!sectores.Any())
        return Results.NoContent();
    return Results.Ok(sectores);
});

// 3.  Actualizar Sector
app.MapPut("/sectores/{sectorID}", ([FromRoute] int sectorID, [FromBody] CrearActualizarSectorDTO sector, [FromServices] ISectorService sectorService) =>
{
    // Validaciones
    var validators = new SectorValidator();
    var result = validators.Validate(sector);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }


    var mostrarSector = sectorService.UpdateSector(sector, sectorID);
    return Results.Ok(mostrarSector);
});

// 4. Eliminar sector
app.MapDelete("/sectores/{sectorID}", ([FromRoute] int sectorID, [FromServices] ISectorService sectorService) =>
{
    // Eliminando sector
    bool fueEliminado = sectorService.DeleteSector(sectorID);

    // Verificación
    if (!fueEliminado)
        return Results.BadRequest();

    // Retornar error
    return Results.Ok(new { message = "Sector eliminado exitosamente." });
});
#endregion

#region Tarifa

app.MapPost("/tarifas", ([FromBody] CrearTarifaDTO tarifa, [FromServices] ITarifaService tarifaService) =>
{
    var validators = new TarifaValidator();
    var result = validators.Validate(tarifa);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }
    var mostrarTarifa =tarifaService.InsertTarifa(tarifa);
    return tarifaService.GetTarifaById(mostrarTarifa.IdTarifa) is null ? Results.BadRequest() : Results.Created($"/tarifas/{mostrarTarifa.IdTarifa}", tarifa);
});

app.MapGet("/funciones/{funcionID}/tarifas", ([FromRoute] int funcionID, [FromServices] ITarifaService tarifaService) =>
{
    var tarifas = tarifaService.GetTarifasByFuncionId(funcionID);
    if (!tarifas.Any())
        return Results.NoContent();
    return Results.Ok(tarifas);
});

app.MapPut("/tarifas/{tarifaID}", ([FromRoute] int tarifaID, [FromBody] ActualizarTarifaDTO tarifa, [FromServices] ITarifaService tarifaService) =>
{
    // Confirmar existencia
    var _tarifa = tarifaService.GetTarifaById(tarifaID);
    if (_tarifa is null)
        return Results.NotFound();

    // Validaciones
    var validators = new ActualizarTarifaDTOValidator();
    var result = validators.Validate(tarifa);
    if (!result.IsValid)
    {
        var listaErrores = result.Errors
        .GroupBy(a => a.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
        return Results.ValidationProblem(listaErrores);
    }

    // Actualizar
    var mostrarTarifa = tarifaService.UpdateTarifa(tarifa, tarifaID);
    return Results.Ok(mostrarTarifa);
});

app.MapGet("/tarifas/{tarifaID}", ([FromRoute] int tarifaID, [FromServices] ITarifaService tarifaService) =>
{
    var tarifa = tarifaService.GetTarifaById(tarifaID);
    if (tarifa is null)
        return Results.NotFound();
    return Results.Ok(tarifa);
});

#endregion

app.Run();