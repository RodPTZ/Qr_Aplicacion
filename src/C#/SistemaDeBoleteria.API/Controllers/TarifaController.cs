using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaDeBoleteria.API.Validators;
using SistemaDeBoleteria.Core;

namespace SistemaDeBoleteria.API.Controllers
{
    [Route("[controller]")]
    public class TarifaController : Controller
    {
        private readonly ILogger<TarifaController> _logger;
        public AdoDapper.Dapper db = new AdoDapper.Dapper();
        public TarifaValidator validators;
        public TarifaController(ILogger<TarifaController> logger)
        {
            _logger = logger;
            db = new AdoDapper.Dapper();
            validators = new TarifaValidator();
        }

        [HttpPost("/tarifas")]
        public IActionResult Post([FromBody] Tarifa tarifa)
        {
            var result = validators.Validate(tarifa);
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

            db.InsertTarifa(tarifa);
            return db.GetTarifaById(tarifa.IdTarifa) is null ? BadRequest() : Created($"/tarifas/{tarifa.IdTarifa}", tarifa);
        }

        [HttpGet("/funcion/{funcionID}/tarifas")]
        public IActionResult Get([FromRoute] int funcionID)
        {
            var _tarifas = db.GetTarifasByFuncionId(funcionID);
            if (!_tarifas.Any())
                return NoContent();
            return Ok(_tarifas);
        }

        [HttpPut("/tarifas/{tarifaID}")]
        public IActionResult Put([FromRoute] int tarifaID, [FromBody] Tarifa tarifa)
        {
            var tarifaFind = db.GetTarifaById(tarifaID);
            if (tarifaFind is null)
                return NotFound();

            var result = validators.Validate(tarifa);
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


            db.UpdateTarifa(tarifa, tarifaID);
            return Ok(tarifaFind);
        }
        [HttpGet("/tarifas/{tarifaID}")]
        public IActionResult GetTarifa([FromRoute] int tarifaID)
        {
            var tarifa = db.GetTarifaById(tarifaID);
            return tarifa is not null ? Ok(tarifa) : NotFound();
        }
    }
}