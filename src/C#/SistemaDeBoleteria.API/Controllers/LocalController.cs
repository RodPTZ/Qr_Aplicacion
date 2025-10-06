using System.Diagnostics;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaDeBoleteria.API.Validators;
using SistemaDeBoleteria.Core;

namespace SistemaDeBoleteria.API.Controllers;

[ApiController]
[Route("[controller]")]
public class LocalController : Controller
{
    private readonly ILogger<LocalController> _logger;
    public AdoDapper.Dapper db;
    public LocalValidator validators;
    public LocalController(ILogger<LocalController> logger)
    {
        _logger = logger;
        db = new AdoDapper.Dapper();
        validators = new LocalValidator();

    }

    [HttpPost("/locales")]
    public IActionResult Post([FromBody] Local local)
    {
        var result = validators.Validate(local);
        if (!result.IsValid)
        {
            var listaErrores = result.Errors
            .GroupBy(a => a.PropertyName)
            .ToDictionary(
                g => g.Key,

                g => g.Select(e => e.ErrorMessage).ToArray()
            );
            return BadRequest(Results.ValidationProblem(listaErrores));
        }

        db.InsertLocal(local);
        return Created($"locales/{local.IdLocal}", local);
    }

    [HttpGet("/locales")]
    public IActionResult Get()
    {
        var locales = db.GetLocales();
        if (!locales.Any())
            return NoContent();
        return Ok(locales);
    }

    [HttpGet("/locales/{localID}")]
    public IActionResult Get([FromRoute] int localID)
    {
        var local = db.GetLocalById(localID);
        return local is null ? NotFound() : Ok(local);
    }

    [HttpPut("/locales/{localID}")]
    public IActionResult Put([FromRoute] int localID, [FromBody] Local local)
    {
        var localFind = db.GetLocalById(localID);
        if (localFind is null)
            return NotFound();

        var result = validators.Validate(local);
        if (!result.IsValid)
        {
            var listaErrores = result.Errors
            .GroupBy(a => a.PropertyName)
            .ToDictionary(
                g => g.Key,

                g => g.Select(e => e.ErrorMessage).ToArray()
            );
            return BadRequest(Results.ValidationProblem(listaErrores));
        }

        db.UpdateLocal(local);
        return Ok(localFind);
    }

    [HttpDelete("/locales/{localID}")]
    public IActionResult Delete([FromRoute] int localID)
    {
        var localDelete = db.GetLocalById(localID);
        if (localDelete is null)
            return NotFound();
        var localEliminado = db.DeleteLocal(localID);
        return localEliminado is false ? BadRequest() : Ok(localDelete);
    }

}
