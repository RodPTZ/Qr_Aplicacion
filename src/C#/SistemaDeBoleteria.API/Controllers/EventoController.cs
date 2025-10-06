using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SistemaDeBoleteria.Core;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using SistemaDeBoleteria.API.Validators;

namespace SistemaDeBoleteria.API.Controllers
{
    [Route("[controller]")]
    public class EventoController : Controller
    {
        private readonly ILogger<EventoController> _logger;

        public AdoDapper.Dapper db;
        public EventoValidator validators;

        public EventoController(ILogger<EventoController> logger)
        {
            _logger = logger;
            db = new AdoDapper.Dapper();
            validators = new EventoValidator();
        }

        [HttpPost("/eventos")]
        public IActionResult Post([FromBody] Evento evento)
        {
            var result = validators.Validate(evento);
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
            db.InsertEvento(evento);
            return db.GetEventoById(evento.IdEvento) is null ? BadRequest() : Created($"/eventos/{evento.IdEvento}", evento);
        }
        [HttpGet("/eventos")]
        public IActionResult Get()
        {
            var eventos = db.GetEventos();
            if (!eventos.Any())
                return NoContent();
            return Ok(eventos);
        }
        [HttpGet("/eventos/{eventoID}")]
        public IActionResult Get(int eventoID)
        {
            var evento = db.GetEventoById(eventoID);
            return evento is null ? NotFound() : Ok(evento);
        }
        [HttpPut("/eventos/{eventoID}")]
        public IActionResult Put([FromRoute] int eventoID, [FromBody] Evento evento)
        {
            if (db.GetEventoById(eventoID) is null)
                return NotFound();

            var result = validators.Validate(evento);
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
            db.InsertEvento(evento);

            evento.IdEvento = eventoID;
            db.UpdateEvento(evento);
            return Ok(evento);
        }

        [HttpPost("/eventos/{eventoID}/publicar")]
        public IActionResult Post([FromRoute] int eventoID)
        {
            if (db.GetEventoById(eventoID) is null)
                return NotFound();
            return db.PublicarEvento(eventoID) is false ? BadRequest() : Ok(db.GetEventoById(eventoID));
        }

        [HttpPost("/eventos/{eventoID}/cancelar")]
        public IActionResult PostC([FromRoute] int eventoID)
        {
            if (db.GetEventoById(eventoID) is null)
                return NotFound();
            db.CancelarEvento(eventoID);
            return Ok(db.GetEventoById(eventoID));
        }
    }
}