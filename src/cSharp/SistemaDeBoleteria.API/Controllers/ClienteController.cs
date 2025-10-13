using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaDeBoleteria.API.Validators;
using SistemaDeBoleteria.Core;


namespace SistemaDeBoleteria.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : Controller
    {
        private readonly ILogger<ClienteController> _logger;

        public AdoDapper.Dapper db { get; private set; }
        public ClienteValidator validators;

        public ClienteController(ILogger<ClienteController> logger)
        {
            _logger = logger;
            db = new AdoDapper.Dapper();
            validators = new ClienteValidator();
        }

        [HttpPost("/clientes")]
        public IActionResult Post([FromBody] Cliente cliente)
        {
            var result = validators.Validate(cliente);
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
            db.InsertCliente(cliente);
            return db.GetClienteById(cliente.IdCliente) is null ? NotFound() : Created($"/clientes/{cliente.IdCliente}", cliente);
        }

        [HttpGet("/clientes")]
        public IActionResult Get()
        {
            var _clientes = db.GetClientes();
            if(!_clientes.Any())
                return NoContent();
            return Ok(_clientes);
        }

        [HttpGet("/clientes/{clienteID}")]
        public IActionResult Get([FromRoute] int clienteID)
        {
            if (db.GetClienteById(clienteID) is null)
                return NotFound();
            return Ok(db.GetClienteById(clienteID));
        }

        [HttpPut("/clientes/{clienteID}")]
        public IActionResult Put([FromRoute] int clienteID, [FromBody] Cliente cliente)
        {
            if (db.GetClienteById(clienteID) is null)
                return NotFound();

            var result = validators.Validate(cliente);
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
            db.UpdateCliente(cliente);
            return Ok(db.GetClienteById(clienteID));
        }
    }
}