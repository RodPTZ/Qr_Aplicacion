using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaDeBoleteria.API.Validators;
using SistemaDeBoleteria.Core;

namespace SistemaDeBoleteria.API.Controllers
{
    [Route("[controller]")]
    public class FuncionController : Controller
    {
        private readonly ILogger<FuncionController> _logger;
        public AdoDapper.Dapper db;
        public FuncionValidator validators;
        public FuncionController(ILogger<FuncionController> logger)
        {
            _logger = logger;
            db = new AdoDapper.Dapper();
            validators = new FuncionValidator();
        }

        [HttpPost("/funciones")]
        public IActionResult Post([FromBody] Funcion funcion)
        {
            var result = validators.Validate(funcion);
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

            db.InsertFuncion(funcion);
            return db.GetFuncionById(funcion.IdFuncion) is null ? BadRequest() : Created($"funciones/{funcion.IdFuncion}", funcion);
        }
        [HttpGet("/funciones")]
        public IActionResult Get()
        {
            var funciones = db.GetFunciones();
            if (!funciones.Any())
                return NoContent();
            return Ok(funciones);
        }
        [HttpGet("/funciones/{funcionID}")]
        public IActionResult Get([FromRoute] int id)
        {
            var funcion = db.GetFuncionById(id);
            if (funcion is null)
                return NotFound();
            return Ok(funcion);
        }
        [HttpPut("/funciones/{funcionID}")]
        public IActionResult Put([FromRoute] int id, [FromBody] Funcion funcion)
        {
            var _funcion = db.GetFuncionById(id);
            if (_funcion is null)
                return NotFound();
            db.UpdateFuncion(funcion, id);
            return db.GetFuncionById(id) is null ? NotFound() : Ok(funcion);
        }
        [HttpPost("/funciones/{funcionID}/cancelar")]
        public IActionResult Post([FromRoute] int id)
        {
            var _funcion = db.GetFuncionById(id);
            if (_funcion is null)
                return NotFound();

            db.CancelarFuncion(id);
            return Ok(_funcion);
        }
    }
}