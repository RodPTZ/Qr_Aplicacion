using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Validations;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Services;
using FluentValidation;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowLocalhost",
//         policy =>
//         {
//             policy.WithOrigins("http://0.0.0.0:5027", "http://localhost:5027", "http://:5027")
//                   .AllowAnyHeader()
//                   .AllowAnyMethod();
//         });
// });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#region Servicios

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICodigoQRService, CodigoQRService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IEntradaService, EntradaService>();
builder.Services.AddScoped<IFuncionService, FuncionService>();
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

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
// app.UseCors("AllowLocalhost");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
    });
}

app.UseHttpsRedirection();

#region Local

app.MapPost("/locales", ([FromBody] CrearActualizarLocalDTO local, [FromServices] ILocalService localService) =>
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
    var localCreado = localService.Post(local);
    //Retornar
    return Results.Created($"/locales/{localCreado.IdLocal}", localCreado);
}).WithTags("A - Locales");

app.MapGet("/locales", ([FromServices] ILocalService localService) =>
{
    // Obtener locales
    var locales = localService.GetAll();
    // Retornar locales
    return !locales.Any() ? Results.NoContent() : Results.Ok(locales);
}).WithTags("A - Locales");

app.MapGet("/locales/{localID}", ([FromRoute] int localID, [FromServices] ILocalService localService) =>
{
    // Obtener local por ID
    var local = localService.Get(localID);
    // Retornar local
    return local is null ? Results.NotFound() : Results.Ok(local);
}).WithTags("A - Locales");

app.MapPut("/locales/{localID}", ([FromRoute] int localID, [FromBody] CrearActualizarLocalDTO local, [FromServices] ILocalService localService) =>
{
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
    var localActualizado = localService.Put(local, localID);
    // Retornar
    return Results.Ok(localActualizado);
}).WithTags("A - Locales");

app.MapDelete("/locales/{localID}", ([FromRoute] int localID, [FromServices] ILocalService localService) =>
{
    bool fueEliminado = localService.Delete(localID);
    if (fueEliminado)
        return Results.Ok(new { mensaje = "Local eliminado correctamente." });

    return Results.BadRequest();
}).WithTags("A - Locales");
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
    var mostrarSector = sectorService.Post(sector, localID);

    // Retornar
    return mostrarSector is null ? Results.BadRequest() : Results.Created($"/locales/{localID}/sectores/{mostrarSector.IdSector}", mostrarSector);
}).WithTags("B - Sector");

// 2. Traer sectores de un local
app.MapGet("/locales/{localID}/sectores", ([FromRoute] int localID, [FromServices] ISectorService sectorService) =>
{
    var sectores = sectorService.GetAllByLocalId(localID);
    return !sectores.Any() ? Results.NoContent() :  Results.Ok(sectores);
}).WithTags("B - Sector");

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


    var mostrarSector = sectorService.Put(sector, sectorID);
    return Results.Ok(mostrarSector);
}).WithTags("B - Sector");

// 4. Eliminar sector
app.MapDelete("/sectores/{sectorID}", ([FromRoute] int sectorID, [FromServices] ISectorService sectorService) =>
{
    // Eliminando sector
    bool fueEliminado = sectorService.Delete(sectorID);
    // Retornar error
    return !fueEliminado ? Results.BadRequest() : Results.Ok(new { message = "Sector eliminado exitosamente." });
}).WithTags("B - Sector");

#endregion

#region Evento

app.MapPost("/eventos", ([FromBody] CrearActualizarEventoDTO evento,[FromServices] IEventoService eventoService) =>
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
    var eventoCreado = eventoService.Post(evento);
    // Retornar
    return Results.Created($"/eventos/{eventoCreado.IdEvento}", eventoCreado);
}).WithTags("C - Eventos");

app.MapGet("/eventos", ([FromServices] IEventoService eventoService) =>
{
    var eventos = eventoService.GetAll();
    return !eventos.Any() ? Results.NoContent() : Results.Ok(eventos);
}).WithTags("C - Eventos");

app.MapGet("/eventos/{eventoID}", ([FromRoute] int eventoID, [FromServices] IEventoService eventoService) =>
{
    // Obtener evento por ID
    var evento = eventoService.GetById(eventoID);
    // Retornar evento
    return evento is null ? Results.NotFound() : Results.Ok(evento);
}).WithTags("C - Eventos");

app.MapPut("/eventos/{eventoID}", ([FromRoute] int eventoID, [FromBody] CrearActualizarEventoDTO evento, [FromServices] IEventoService eventoService) =>
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
    // Actualizar
    var eventoActualizado = eventoService.Put(evento, eventoID);
    return eventoActualizado is null ? Results.NotFound() : Results.Ok(eventoActualizado);
}).WithTags("C - Eventos");

app.MapPost("/eventos/{eventoID}/publicar", ([FromRoute] int eventoID, [FromServices] IEventoService eventoService) =>
{
    // Publicar
    var operacion = eventoService.PublicarEvento(eventoID);

    return operacion.caso switch
    {
        1 => Results.Ok(new { message = operacion.Message }),
        2 => Results.BadRequest(new { message = operacion.Message }),
        3 => Results.NotFound(new { message = operacion.Message})
    };
}).WithTags("C - Eventos");

app.MapPost("/eventos/{eventoID}/cancelar", ([FromRoute] int eventoID, [FromServices] IEventoService eventoService) =>
{
    eventoService.CancelarEvento(eventoID); // falta verificar la salida.

}).WithTags("C - Eventos");
#endregion
// Evento: Cancelar (falta completar)
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
    var funcionCreada = funcionService.Post(funcion);

    // Retornar 
    return Results.Created($"/funciones/{funcionCreada.IdFuncion}", funcionCreada);
}).WithTags("D - Funciones");

app.MapGet("/funciones", ([FromServices] IFuncionService funcionService) =>
{
    // Obtener funciones
    var funciones = funcionService.GetAll();
    // Retornar funciones
    return !funciones.Any() ? Results.NoContent() : Results.Ok(funciones);
}).WithTags("D - Funciones");

app.MapGet("/funciones/{funcionID}", ([FromRoute] int funcionID, [FromServices] IFuncionService funcionService) =>
{
    // Verificar existencia
    var funcion = funcionService.Get(funcionID);
    // Retornar funcion
    return funcion is null ? Results.NotFound() : Results.Ok(funcion);
}).WithTags("D - Funciones");

app.MapPut("/funciones/{funcionID}", ([FromRoute] int funcionID, [FromBody] ActualizarFuncionDTO funcion, [FromServices] IFuncionService funcionService) =>
{
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
    var funcionActualizada = funcionService.Put(funcion, funcionID);
    // Retornar
    return Results.Ok(funcionActualizada);
}).WithTags("D - Funciones");

app.MapPost("/funciones/{funcionID}/cancelar", ([FromRoute] int funcionID, [FromServices] IFuncionService funcionService) =>
{
    funcionService.Cancelar(funcionID);

    // Retornar
    return Results.Ok();
}).WithTags("D - Funciones");
#endregion
// Funcion: Cancelar() : Falta validar salida
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

    var mostrarTarifa = tarifaService.Post(tarifa);

    return Results.Created($"/tarifas/{mostrarTarifa.IdTarifa}", mostrarTarifa);
}).WithTags("E - Tarifas");

app.MapGet("/funciones/{funcionID}/tarifas", ([FromRoute] int funcionID, [FromServices] ITarifaService tarifaService) =>
{
    var tarifas = tarifaService.GetAllByFuncionId(funcionID);

    return !tarifas.Any() ?  Results.NoContent() : Results.Ok(tarifas);
}).WithTags("E - Tarifas");

app.MapPut("/tarifas/{tarifaID}", ([FromRoute] int tarifaID, [FromBody] ActualizarTarifaDTO tarifa, [FromServices] ITarifaService tarifaService) =>
{
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
    var mostrarTarifa = tarifaService.Put(tarifa, tarifaID);
    return Results.Ok(mostrarTarifa);
}).WithTags("E - Tarifas");

app.MapGet("/tarifas/{tarifaID}", ([FromRoute] int tarifaID, [FromServices] ITarifaService tarifaService) =>
{
    var tarifa = tarifaService.Get(tarifaID);

    return tarifa is null ? Results.NotFound() : Results.Ok(tarifa);
}).WithTags("E - Tarifas");

#endregion

#region Cliente

app.MapPost("/clientes", ([FromBody] CrearClienteDTO cliente, [FromServices] IClienteService clienteService) =>
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

    var mostrarClienteDTO = clienteService.Post(cliente);
    return Results.Created($"/clientes/{mostrarClienteDTO.IdCliente}", mostrarClienteDTO);
}).WithTags("F - Clientes");
// ^_ Aprobado

app.MapGet("/clientes", (IClienteService clienteService) =>
{
    var _clientes = clienteService.GetAll();
    if (!_clientes.Any())
        return Results.NoContent();

    return Results.Ok(_clientes);
}).WithTags("F - Clientes");
// Aprob
app.MapGet("/clientes/{clienteID}", ([FromRoute] int clienteID, IClienteService clienteService) =>
{
    var cliente = clienteService.GetById(clienteID);
    if (cliente is null)
        return Results.NotFound();

    return Results.Ok(cliente);
    // return clienteService.GetById(clienteID) is null ? Results.NotFound() : Results.Ok(clienteService.GetById(clienteID)); 

}).WithTags("F - Clientes");

app.MapPut("/clientes/{clienteID}", ([FromRoute] int clienteID, [FromBody] ActualizarClienteDTO cliente, [FromServices] IClienteService clienteService, [FromServices] IValidator<ActualizarClienteDTO> validator) =>
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
}).WithTags("F - Clientes");
#endregion
// Cliente terminado, falta llamar a las validaciones mediante [FromService]
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
    var ordenCreada = ordenService.Post(orden);

    // Retornar 
    return ordenCreada is null ? Results.BadRequest() : Results.Created($"/ordenes/{ordenCreada.IdOrden}", ordenCreada);
}).WithTags("G - Orden");

app.MapGet("/ordenes", ([FromServices] IOrdenService ordenService) =>
{
    var ordenes = ordenService.GetAll();
    return !ordenes.Any() ? Results.NoContent() : Results.Ok(ordenes) ;
}).WithTags("G - Orden");

app.MapGet("/ordenes/{ordenID}", ([FromRoute] int ordenID, [FromServices] IOrdenService ordenService) =>
{
    var orden = ordenService.Get(ordenID);

    return orden is null ? Results.NotFound() : Results.Ok(orden) ;
}).WithTags("G - Orden");

app.MapPut("/ordenes/{ordenID}/pagar", ([FromRoute] int ordenID, [FromServices] IOrdenService ordenService) =>
{

    var fuePagado = ordenService.PagarOrden(ordenID);
    return fuePagado is true ? Results.Ok(new { message = "Pagado Exitosamente." }) : Results.BadRequest();
}).WithTags("G - Orden");

app.MapPut("/ordenes/{ordenID}/cancelar", ([FromRoute] int ordenID, [FromServices] IOrdenService ordenService) =>
{
    bool fueCancelado = ordenService.CancelarOrden(ordenID);
    // Retornar
    return fueCancelado is true ? Results.Ok(new { message = "Cancelado Exitosamente." }) : Results.BadRequest();
}).WithTags("G - Orden");

#endregion

#region  Entrada

app.MapGet("/entradas", ([FromServices] IEntradaService entradaService) =>
{
    var entradas = entradaService.GetAll();
    return !entradas.Any() ? Results.NoContent() : Results.Ok(entradas);
}).WithTags("H - Entradas");

app.MapGet("/entradas/{entradaID}", ([FromRoute] int entradaID, [FromServices] IEntradaService entradaService) =>
{
    var entrada = entradaService.GetById(entradaID);
    return entrada is null ? Results.NotFound() : Results.Ok(entrada);
}).WithTags("H - Entradas");

app.MapPost("/entradas/{entradaID}/anular", ([FromRoute] int entradaID, [FromServices] IEntradaService entradaService) =>
{
    bool funciono = entradaService.AnularEntrada(entradaID);
    return funciono is false ? Results.NotFound(new { message = "No funciona" }) : Results.Ok(new { message = "Si funciona" });
}).WithTags("H - Entradas");

#endregion

#region CodigoQR

app.MapGet("/entradas/{entradaID}/qr", ([FromRoute] int entradaID, [FromServices] ICodigoQRService codigoQRService, [FromServices] IEntradaService entradaService) =>
{
    var QrPng = codigoQRService.GetQRByEntradaId(entradaID);
    return QrPng is null ? Results.NotFound() : Results.File(QrPng, "image/png");
}).WithTags("I - CodigoQR");

app.MapPost("/qr/validar", ([FromQuery] int idEntrada, [FromQuery] string Codigo, [FromServices] ICodigoQRService codigoQRService) =>
{
    var estado = codigoQRService.ValidateQR(idEntrada, Codigo);
    return Results.Ok(new { Estado = estado});

}).WithTags("I - CodigoQR");

#endregion

#region Login

app.MapPost("/register", ([FromBody] RegisterRequest register, [FromServices] ILoginService loginService) =>
{
    var usuario = loginService.Register(register);
    return Results.Created($"/register/{usuario.Email}", usuario);
}).WithTags("J - Login");
app.MapPost("/login", () =>
{

}).WithTags("J - Login");
app.MapPost("/refresh", () =>
{

}).WithTags("J - Login");
app.MapPost("/logout", () =>
{

}).WithTags("J - Login");
app.MapGet("/me", () =>
{

}).WithTags("J - Login");
app.MapGet("/roles", () =>
{
    string[] roles = {
        "Administrador",
        "Organizador",
        "Cliente",
        "Control de Acceso"
    };
    return Results.Ok(roles);
}).WithTags("J - Login");
app.MapPost("/usuarios/{usuarioID}/roles", ([FromRoute] int usuarioID, [FromServices] ILoginService loginService) =>
{
    return Results.Ok();
}).WithTags("J - Login");
#endregion

app.Run();
 