using System.Security.Claims;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace SistemaDeBoleteria.API.Endpoints
{
    public static class LoginEndpoints
    {
        public static void MapLoginEndpoints(this WebApplication app)
        {
            app.MapPost("/register", 
                ([FromBody] RegisterRequest register, 
                 [FromServices] ILoginService loginService, 
                 [FromServices] IValidator<RegisterRequest> validators) =>
                {
                    var result = validators.Validate(register);
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

                    var usuario = loginService.Register(register);
                    return Results.Created($"/register/{usuario.Email}", usuario);
                })
                .WithTags("J - Login")
                .RequireAuthorization("Admin");
            app.MapPost("/login",
                ([FromBody] LoginRequest loginRequest,
                 [FromServices] ILoginService loginService,
                 [FromServices] IValidator<LoginRequest> validators) =>
                {
                    var result = validators.Validate(loginRequest);
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

                    var loginResponse = loginService.Login(loginRequest);
                    return Results.Ok(loginResponse);
                })
                .WithTags("J - Login");

            app.MapPost("/refresh", 
                (string refreshToken, [FromServices] ILoginService loginService) =>
                {
                    var newToken = loginService.RefreshToken(refreshToken);
                    return Results.Ok(new { Token = newToken });
                })
                .WithTags("J - Login")
                .RequireAuthorization();

            app.MapPost("/logout", 
                ([FromBody] string refreshToken, [FromServices] ILoginService loginService) =>
                {
                    var result = loginService.Logout(refreshToken);
                    return result
                        ? Results.Ok(new { message = "Cierre de sesión exitoso." })
                        : Results.BadRequest(new { message = "Error al cerrar sesión. Refresh Token inválido." });
                })
                .WithTags("J - Login")
                .RequireAuthorization();

            app.MapGet("/me", 
                (HttpContext httpContext, [FromServices] ILoginService loginService) =>
                {
                    var email = httpContext.User.FindFirstValue(ClaimTypes.Email)!;
                    var usuario = loginService.Me(email);
                    return usuario is null ? Results.NotFound() : Results.Ok(usuario);
                })
                .WithTags("J - Login")
                .RequireAuthorization();

            app.MapGet("/roles", () =>
            {
                var roles = Enum.GetValues<ERolUsuario>()
                                .ToDictionary(rol => rol.ToString(), rol => (int)rol);
                return Results.Ok(roles);
                
            })
            .WithTags("J - Login")
            .RequireAuthorization("Admin");

            app.MapPost("/usuarios/{usuarioID}/roles",
                ([FromRoute] int usuarioID, [FromBody] ERolUsuario rol, [FromServices] ILoginService loginService) =>
                {
                    loginService.ChangeRol(usuarioID, rol.ToString());
                    return Results.Ok("Cambio de rol exitoso.");
                })
                .WithTags("J - Login")
                .RequireAuthorization("Admin");
        }
    }
}
