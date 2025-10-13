using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Logging;
using SistemaDeBoleteria.Core;

namespace SistemaDeBoleteria.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CodigoQRController : Controller
{
    private readonly ILogger<CodigoQRController> _logger;
    public AdoDapper.Dapper db = new AdoDapper.Dapper();

    public CodigoQRController(ILogger<CodigoQRController> logger)
    {
        _logger = logger;
        db = new AdoDapper.Dapper();
    }

    [HttpGet("/entradas/{entradaID}/qr")]
    public IActionResult Get([FromRoute] int entradaID)
    {
        var entrada = db.GetEntradaById(entradaID);
        if (entrada is null)
            return NotFound();
        var Qr = db.GetQRByEntradaId(entradaID);
        return Ok(Qr);
    }

    [HttpPost("/qr/validar")]
    public IActionResult PostValidar([FromBody] int IdEntrada)
    {
        var entrada = db.GetEntradaById(IdEntrada);
        if (entrada is null)
            return NotFound();
            
        db.ValidarQR(IdEntrada);        
        return Ok(entrada.QR.Estado);
    }
}
