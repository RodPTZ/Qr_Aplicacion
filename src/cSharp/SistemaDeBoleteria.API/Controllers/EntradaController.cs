using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaDeBoleteria.Core;
namespace SistemaDeBoleteria.API.Controllers
{
    [Route("[controller]")]
    public class EntradaController : Controller
    {
        private readonly ILogger<EntradaController> _logger;
        public AdoDapper.Dapper db;

        public EntradaController(ILogger<EntradaController> logger)
        {
            _logger = logger;
            db = new AdoDapper.Dapper();
        }

        [HttpGet("/entradas")]
        public IActionResult Get()
        {
            var entradas = db.GetEntradas();
            if (!entradas.Any())
                return NotFound();
            return Ok(entradas);
        }
        [HttpGet("/entradas/{entradaID}")]
        public IActionResult Get([FromRoute] int entradaID)
        {
            var entrada = db.GetEntradaById(entradaID);
            return entrada is not null ? Ok(entrada) : NotFound();
        }
        [HttpPost("/entradas/{entradaID}/anular")]
        public IActionResult Post([FromRoute] int entradaID)
        {
            var entrada = db.GetEntradaById(entradaID);
            if (entrada is null)
                return NotFound();
            db.AnularEntrada(entradaID);
            return Ok(entrada);
        }
    }
}