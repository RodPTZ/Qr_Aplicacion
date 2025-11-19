using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using SistemaDeBoleteria.Core.Exceptions;

namespace SistemaDeBoleteria.API.Middleware
{
    public class ExceptionsMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionsMiddleware(RequestDelegate _next)
        {
            this._next = _next; 
        }
    
       public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
{
    httpContext.Response.ContentType = "application/json";

    string message = exception.Message;

    HttpStatusCode statusCode = exception switch
    {
        NotFoundException => HttpStatusCode.NotFound,
        BusinessException => HttpStatusCode.BadRequest,
        NoContentException => HttpStatusCode.BadRequest,
        DataBaseException => HttpStatusCode.InternalServerError,
        _ => HttpStatusCode.InternalServerError
    };

    httpContext.Response.StatusCode = (int)statusCode;

    var result = JsonSerializer.Serialize(new { error = message, type = exception.GetType().Name });

    await httpContext.Response.WriteAsync(result);
}

    }
}