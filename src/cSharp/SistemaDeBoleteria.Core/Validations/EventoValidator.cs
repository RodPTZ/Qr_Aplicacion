using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
using FluentValidation;
using SistemaDeBoleteria.Core.DTOs;
namespace SistemaDeBoleteria.Core.Validations
{
    public class EventoValidator : AbstractValidator<CrearActualizarEventoDTO>
    {
        public EventoValidator()
        {
            RuleFor(e => e.IdLocal)
                .GreaterThan(0).WithMessage("Debe ser mayor que 0");
            RuleFor(e => e.Nombre)
                .NotEmpty().WithMessage("El nombre no puede estar vacío")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres");
            RuleFor(e => e.Tipo)
                .IsInEnum().WithMessage("Tipo de evento inválido");
        }
    }
}