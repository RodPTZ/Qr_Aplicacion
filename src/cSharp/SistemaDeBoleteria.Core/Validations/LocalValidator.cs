using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
using FluentValidation;
using SistemaDeBoleteria.Core.DTOs;
using System.Data;
namespace SistemaDeBoleteria.Core.Validations
{
    public class LocalValidator : AbstractValidator<CrearActualizarLocalDTO>
    {
        public LocalValidator()
        {
            RuleFor(l => l.Nombre)
                .NotEmpty().WithMessage("El nombre no puede estar vacío");
            RuleFor(l => l.Ubicacion)
                .MinimumLength(4).WithMessage("La ubicación debe más de 3 caracteres.");
        }
    }
}