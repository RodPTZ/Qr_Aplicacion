using Microsoft.AspNetCore.Mvc;
using SistemaDeBoleteria.AdoDapper;
using SistemaDeBoleteria.Core;
using SistemaDeBoleteria.Core.Services;
using SistemaDeBoleteria.Core.Validations;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IClienteService, ClienteAdo>();
builder.Services.AddScoped<ICodigoQRService, CodigoQRAdo>();
builder.Services.AddScoped<IEventoService, EventoAdo>();
builder.Services.AddScoped<ILocalService, LocalAdo>();
builder.Services.AddScoped<IOrdenService, OrdenAdo>();
builder.Services.AddScoped<ISectorService, SectorAdo>();
builder.Services.AddScoped<ITarifaService, TarifaAdo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Cliente

app.MapPost("/clientes", ([FromBody] Cliente cliente, IClienteService clienteService) =>
{
    var validator = new ClienteValidator();
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
    clienteService.InsertCliente(cliente);
    return clienteService.GetClienteById(cliente.IdCliente) is null ? Results.NotFound() : Results.Created($"/clientes/{cliente.IdCliente}", cliente);
}).WithTags("Clientes");

app.MapGet("/clientes", (IClienteService clienteService) =>
{
    var _clientes = clienteService.GetClientes();
    if (!_clientes.Any())
        return Results.NoContent();
    return Results.Ok(_clientes);
}).WithTags("Clientes");

app.MapGet("/clientes/{clienteID}", ([FromRoute] int clienteID, IClienteService clienteService) =>
{
    var _cliente = clienteService.GetClienteById(clienteID);
    if (_cliente is null)
        return Results.NotFound();
    return Results.Ok(_cliente);
}).WithTags("Clientes");

app.MapPut("/clientes/{clienteID}", ([FromRoute] int clienteID, [FromBody] Cliente cliente, IClienteService clienteService) =>
{
    var _cliente = clienteService.GetClienteById(clienteID);
    if (_cliente is null)
        return Results.NotFound();

    var validators = new ClienteValidator();
    var result = validators.Validate(cliente);
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
    clienteService.UpdateCliente(cliente);
    return Results.Ok(_cliente);
}).WithTags("Clientes");
#endregion

#region CodigoQR



app.MapGet("/entradas/{entradaID}/qr", ([FromRoute] int entradaID, [FromServices] ICodigoQRService codigoQRService, [FromServices] IEntradaService entradaService) =>
{
    var entrada = entradaService.GetEntradaById(entradaID);
    if (entrada is null)
        return Results.NotFound();
    var Qr = codigoQRService.GetQRByEntradaId(entradaID);
    return Results.Ok(Qr);
}).WithTags("CodigoQR");

app.MapPost("/qr/validar", ([FromBody] int IdEntrada, [FromServices] IEntradaService entradaService, [FromServices] ICodigoQRService codigoQRService) =>
{
    var entrada = entradaService.GetEntradaById(IdEntrada);
    if (entrada is null)
        return Results.NotFound();

    codigoQRService.ValidarQR(IdEntrada);
    return Results.Ok(entrada.QR.Estado);
}).WithTags("CodigoQR");
#endregion

#region  Entrada

app.MapGet("/entradas", ([FromServices] IEntradaService entradaService) =>
{
    var entradas = entradaService.GetEntradas();
    if (!entradas.Any())
        return Results.NoContent();
    return Results.Ok(entradas);
}).WithTags("Entradas");

app.MapGet("/entradas/{entradaID}", ([FromRoute] int entradaID, [FromServices] IEntradaService entradaService) =>
{
    var entrada = entradaService.GetEntradaById(entradaID);
    return entrada is not null ? Results.Ok(entrada) : Results.NotFound();
}).WithTags("Entradas");

app.MapPost("/entradas/{entradaID}/anular", ([FromRoute] int entradaID, [FromServices] IEntradaService entradaService) =>
{
    var entrada = entradaService.GetEntradaById(entradaID);
    if (entrada is null)
        return Results.NotFound();
    entradaService.AnularEntrada(entradaID);
    return Results.Ok(entrada);
}).WithTags("Entradas");
#endregion

#region  Evento

app.MapPost("/eventos", ([FromBody] Evento evento, [FromServices] IEventoService eventoService) =>
{
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
    eventoService.InsertEvento(evento);
    return eventoService.GetEventoById(evento.IdEvento) is null ? Results.BadRequest() : Results.Created($"/eventos/{evento.IdEvento}", evento);
}).WithTags("Eventos");

app.MapGet("/eventos", ([FromServices] IEventoService eventoService) =>
{
    var eventos = eventoService.GetEventos();
    if (!eventos.Any())
        return Results.NoContent();
    return Results.Ok(eventos);
}).WithTags("Eventos");

app.MapGet("/eventos/{eventoID}", ([FromRoute] int eventoID, [FromServices] IEventoService eventoService) =>
{
    var evento = eventoService.GetEventoById(eventoID);
    return evento is not null ? Results.Ok(evento) : Results.NotFound();
}).WithTags("Eventos");

app.MapPut("/eventos/{eventoID}", ([FromRoute] int eventoID, [FromBody] Evento evento, [FromServices] IEventoService eventoService) =>
{
    var _evento = eventoService.GetEventoById(eventoID);
    if (_evento is null)
        return Results.NotFound();

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
    eventoService.UpdateEvento(evento);
    return Results.Ok(_evento);
}).WithTags("Eventos");

app.MapPost("/eventos/{eventoID}/publicar", ([FromRoute] int eventoID, [FromServices] IEventoService eventoService) =>
{
    var evento = eventoService.GetEventoById(eventoID);
    if (evento is null)
        return Results.NotFound();

    if (eventoService.PublicarEvento(eventoID))
        return Results.Ok(evento);

    return Results.BadRequest();
}).WithTags("Eventos");

app.MapPost("/eventos/{eventoID}/cancelar", ([FromRoute] int eventoID, [FromServices] IEventoService eventoService) =>
{
    var evento = eventoService.GetEventoById(eventoID);
    if (evento is null)
        return Results.NotFound();

    if (eventoService.CancelarEvento(eventoID))
        return Results.Ok(evento);

    return Results.BadRequest();
}).WithTags("Eventos");
#endregion

#region Funcion

app.MapPost("/funciones", ([FromBody] Funcion funcion, [FromServices] IFuncionService funcionService) =>
{
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
    funcionService.InsertFuncion(funcion);
    return funcionService.GetFuncionById(funcion.IdFuncion) is null ? Results.BadRequest() : Results.Created($"/funciones/{funcion.IdFuncion}", funcion);
}).WithTags("Funciones");

app.MapGet("/funciones", ([FromServices] IFuncionService funcionService) =>
{
    var funciones = funcionService.GetFunciones();
    if (!funciones.Any())
        return Results.NoContent();
    return Results.Ok(funciones);
}).WithTags("Funciones");

app.MapGet("/funciones/{funcionID}", ([FromRoute] int funcionID, [FromServices] IFuncionService funcionService) =>
{
    var funcion = funcionService.GetFuncionById(funcionID);
    return funcion is not null ? Results.Ok(funcion) : Results.NotFound();
}).WithTags("Funciones");

app.MapPut("/funciones/{funcionID}", ([FromRoute] int funcionID, [FromBody] Funcion funcion, [FromServices] IFuncionService funcionService) =>
{
    var _funcion = funcionService.GetFuncionById(funcionID);
    if (_funcion is null)
        return Results.NotFound();

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
    funcionService.UpdateFuncion(funcion, funcionID);
    return Results.Ok(_funcion);
}).WithTags("Funciones");

app.MapPost("/funciones/{funcionID}/cancelar", ([FromRoute] int funcionID, [FromServices] IFuncionService funcionService) =>
{
    var funcion = funcionService.GetFuncionById(funcionID);
    if (funcion is null)
        return Results.NotFound();

    funcionService.CancelarFuncion(funcionID);
    return Results.Ok(funcion);
}).WithTags("Funciones");
#endregion

#region Local

app.MapPost("/locales", ([FromBody] Local local, [FromServices] ILocalService localeService) =>
{
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
    localeService.InsertLocal(local);
    return localeService.GetLocalById(local.IdLocal) is null ? Results.BadRequest() : Results.Created($"/locales/{local.IdLocal}", local);
}).WithTags("Locales");

app.MapGet("/locales", ([FromServices] ILocalService localeService) =>
{
    var locales = localeService.GetLocales();
    if (!locales.Any())
        return Results.NoContent();
    return Results.Ok(locales);
}).WithTags("Locales");
app.MapGet("/locales/{localID}", ([FromRoute] int localID, [FromServices] ILocalService localeService) =>
{
    var local = localeService.GetLocalById(localID);
    return local is not null ? Results.Ok(local) : Results.NotFound();
}).WithTags("Locales");

app.MapPut("/locales/{localID}", ([FromRoute] int localID, [FromBody] Local local, [FromServices] ILocalService localeService) =>
{
    var _local = localeService.GetLocalById(localID);
    if (_local is null)
        return Results.NotFound();

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
    localeService.UpdateLocal(local);
    return Results.Ok(_local);
}).WithTags("Locales");

app.MapDelete("/locales/{localID}", ([FromRoute] int localID, [FromServices] ILocalService localeService) =>
{
    var local = localeService.GetLocalById(localID);
    if (local is null)
        return Results.NotFound();

    if (localeService.DeleteLocal(localID))
        return Results.Ok(local);

    return Results.BadRequest();
}).WithTags("Locales");
#endregion

#region Login

app.MapPost("/register", ([FromBody] Usuario usuario, [FromServices] ILoginService loginService) =>
{
    loginService.AuthRegistrar(usuario);
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
    var roles = loginService.GetRolesByUserId(usuarioID);
    if (!roles.Any())
        return Results.NoContent();
    return Results.Ok(roles);
}).WithTags("Login");
#endregion

#region Orden

app.MapPost("/ordenes", ([FromBody] Orden orden, [FromServices] IOrdenService ordenService) =>
{
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
    ordenService.InsertOrden(orden);
    return ordenService.GetOrdenById(orden.IdOrden) is null ? Results.BadRequest() : Results.Created($"/ordenes/{orden.IdOrden}", orden);
});

app.MapGet("/ordenes", ([FromServices] IOrdenService ordenService) =>
{
    var ordenes = ordenService.GetOrdenes();
    if (!ordenes.Any())
        return Results.NoContent();
    return Results.Ok(ordenes);
});

app.MapGet("/ordenes/{ordenID}", ([FromRoute] int ordenID, [FromServices] IOrdenService ordenService) =>
{
    var orden = ordenService.GetOrdenById(ordenID);
    return orden is not null ? Results.Ok(orden) : Results.NotFound();
});

app.MapPost("/ordenes/{ordenID}/pagar", ([FromRoute] int ordenID, [FromServices] IOrdenService ordenService) =>
{
    var orden = ordenService.GetOrdenById(ordenID);
    if (orden is null)
        return Results.NotFound();

    ordenService.PagarOrden(ordenID);
    return Results.Ok(orden);
});

app.MapPost("/ordenes/{ordenID}/cancelar", ([FromRoute] int ordenID, [FromServices] IOrdenService ordenService) =>
{
    var orden = ordenService.GetOrdenById(ordenID);
    if (orden is null)
        return Results.NotFound();

    ordenService.CancelarOrden(ordenID);
    return Results.Ok(orden);
});

#endregion

#region Sector

app.MapPost("/locales/{localID}/sectores", ([FromRoute] int localID, [FromBody] Sector sector, [FromServices] ISectorService sectorService) =>
{
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
    sectorService.InsertSector(sector, localID);
    return sectorService.GetSectorByLocalId(sector.IdSector) is null ? Results.BadRequest() : Results.Created($"/locales/{localID}/sectores/{sector.IdSector}", sector);
});

app.MapGet("/locales/{localID}/sectores", ([FromRoute] int localID, [FromServices] ISectorService sectorService) =>
{
    var sectores = sectorService.GetSectoresByLocalId(localID);
    if (!sectores.Any())
        return Results.NoContent();
    return Results.Ok(sectores);
});

app.MapPut("/sectores/{sectorID}", ([FromRoute] int sectorID, [FromBody] Sector sector, [FromServices] ISectorService sectorService) =>
{
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
    sectorService.UpdateSector(sector, sectorID);
    return Results.Ok(sector);
});

app.MapDelete("/sectores/{sectorID}", ([FromRoute] int sectorID, [FromServices] ISectorService sectorService) =>
{
    var sector = sectorService.GetSectorById(sectorID);
    if (sector is null)
        return Results.NotFound();

    if (sectorService.DeleteSector(sectorID))
        return Results.Ok(sector);

    return Results.BadRequest();
});
#endregion

#region Tarifa

app.MapPost("/tarifas", ([FromBody] Tarifa tarifa, [FromServices] ITarifaService tarifaService) =>
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
    tarifaService.InsertTarifa(tarifa);
    return tarifaService.GetTarifaById(tarifa.IdTarifa) is null ? Results.BadRequest() : Results.Created($"/tarifas/{tarifa.IdTarifa}", tarifa);
});

app.MapGet("/funciones/{funcionID}/tarifas", ([FromRoute] int funcionID, [FromServices] ITarifaService tarifaService) =>
{
    var tarifas = tarifaService.GetTarifasByFuncionId(funcionID);
    if (!tarifas.Any())
        return Results.NoContent();
    return Results.Ok(tarifas);
});

app.MapPut("/tarifas/{tarifaID}", ([FromRoute] int tarifaID, [FromBody] Tarifa tarifa, [FromServices] ITarifaService tarifaService) =>
{
    var _tarifa = tarifaService.GetTarifaById(tarifaID);
    if (_tarifa is null)
        return Results.NotFound();

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
    tarifaService.UpdateTarifa(tarifa, tarifaID);
    return Results.Ok(_tarifa);
});

app.MapGet("/tarifas/{tarifaID}", ([FromRoute] int tarifaID, [FromServices] ITarifaService tarifaService) =>
{
    var tarifa = tarifaService.GetTarifaById(tarifaID);
    if (tarifa is null)
        return Results.NotFound();
    return Results.Ok(tarifa);
});

#endregion
// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };
// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();
app.Run();
// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
