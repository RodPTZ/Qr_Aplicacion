using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SistemaDeBoleteria.Core;
namespace SistemaDeBoleteria.API.Validators
{
    public class EventoValidator : AbstractValidator<Evento>
    {
        public EventoValidator()
        {
            RuleFor(e => e.IdLocal)
                .GreaterThan(0).WithMessage("Debe ser mayor que 0");
            RuleFor(e => e.Nombre)
                .NotEmpty().WithMessage("El nombre no puede estar ")
                .MaximumLength(10).WithMessage("asd");
        }
    }
}