using System.Data.Common;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Logging;
using SistemaDeBoleteria.API.Validators;
using SistemaDeBoleteria.Core;
namespace SistemaDeBoleteria.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SectorController : Controller
{
    private readonly ILogger<SectorController> _logger;

    public AdoDapper.Dapper db;
    public SectorValidator validators;
    public SectorController(ILogger<SectorController> logger)
    {
        _logger = logger;
        db = new AdoDapper.Dapper();
        validators = new SectorValidator();
    }

    [HttpPost("/locales/{localID}/sectores")]
    public IActionResult Post([FromRoute] int localID, [FromBody] Sector sector)
    {
        var local = db.GetLocalById(localID);
        if (local is null)
            return NotFound();

        var result = validators.Validate(sector);
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
        
        db.InsertSector(sector, localID);
        return Created($"/locales/{localID}/sectores/{sector.IdSector}", sector);
    }
    [HttpGet("/locales/{localID}/sectores")]
    public IActionResult Get([FromRoute] int localID)
    {
        var local = db.GetLocalById(localID);
        if (local is null)
            return NotFound();

        var sectoresDe = db.GetSectoresByLocalId(localID);
        if (!sectoresDe.Any())
            return NoContent();

        return Ok(sectoresDe);
    }

    
    [HttpPut("/sectores/{sectorID}")]
    public IActionResult Put([FromRoute] int sectorID, [FromBody] Sector sector)
    {
        var sectorFind = db.GetSectorById(sectorID);
        if (sectorFind is null)
            return NotFound();

        var actualizado = db.UpdateSector(sector, sectorID);
        return actualizado is true ? Ok(sectorFind) : BadRequest();
    }
    
    [HttpDelete("/sectores/{sectorID}")]
    public IActionResult Delete([FromRoute] int sectorID)
    {
        var sector = db.GetSectorById(sectorID);
        if (sector is null)
            return NotFound();

        var eliminado = db.DeleteSector(sectorID);
        return eliminado is true ? Ok(sector) : BadRequest();
    }
}
