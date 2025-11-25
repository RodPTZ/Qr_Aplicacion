using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SistemaDeBoleteria.API.Endpoints;
using SistemaDeBoleteria.API.Extensions;
using SistemaDeBoleteria.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Empleado", policy => policy.RequireRole("Empleado", "Admin"));
    options.AddPolicy("Organizador", policy => policy.RequireRole("Organizador", "Admin"));
    options.AddPolicy("Cliente", policy => policy.RequireRole("Cliente", "Admin"));
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));

    options.AddPolicy("EmpleadoOrganizador", policy => policy.RequireRole("Empleado", "Organizador", "Admin"));
    options.AddPolicy("ClienteOrganizador", policy => policy.RequireRole("Cliente", "Organizador", "Admin"));

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
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

builder.Services.AddRepositories(builder.Configuration);

builder.Services.AddServices();

builder.Services.AddValidations();

var app = builder.Build();

app.UseMiddleware<ExceptionsMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
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

app.MapLocalEndpoints();

app.MapSectorEndpoints();

app.MapEventoEndpoints();

app.MapFuncionEndpoints();

app.MapTarifaEndpoints();

app.MapClienteEndpoints();

app.MapOrdenEndpoints();

app.MapEntradaEndpoints();

app.MapCodigoQREndpoints();

app.MapLoginEndpoints();

app.Run();
 