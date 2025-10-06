using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaDeBoleteria.API.Validators;
using SistemaDeBoleteria.Core;

namespace SistemaDeBoleteria.API.Controllers
{
    [Route("[controller]")]
    public class OrdenController : Controller
    {
        private readonly ILogger<OrdenController> _logger;
        private readonly AdoDapper.Dapper db;
        public OrdenValidator validators;
        public OrdenController(ILogger<OrdenController> logger)
        {
            _logger = logger;
            db = new AdoDapper.Dapper();
            validators = new OrdenValidator();
        }

        [HttpPost("/ordenes")]
        public IActionResult Post([FromBody] Orden orden)
        {
            var result = validators.Validate(orden);
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

            db.InsertOrden(orden);
            return Created($"/ordenes/{orden.IdOrden}", orden);
        }

        [HttpGet("/ordenes")]
        public IActionResult Get()
        {
            var ordenes = db.GetOrdenes();
            if (!ordenes.Any())
                return NoContent();
            return Ok(ordenes);
        }

        [HttpGet("/ordenes/{ordenID}")]
        public IActionResult GetOrden([FromRoute] int ordenID)
        {
            var orden = db.GetOrdenById(ordenID);
            return orden is null ? NotFound() : Ok(orden);
        }

        [HttpPost("/ordenes/{ordenID}/pagar")]
        public IActionResult PostPagar([FromRoute] int ordenID)
        {
            var orden = db.GetOrdenById(ordenID);
            if (orden is null)
                return NotFound();

            db.PagarOrden(ordenID);
            return Ok(orden);
        }

        [HttpPost("/ordenes/{ordenID}/cancelar")]
        public IActionResult PostCancelar([FromRoute] int ordenID)
        {
            var orden = db.GetOrdenById(ordenID);
            if (orden is null)
                return NotFound();
            db.CancelarOrden(ordenID);
            return Ok(orden);
        }
    }
}