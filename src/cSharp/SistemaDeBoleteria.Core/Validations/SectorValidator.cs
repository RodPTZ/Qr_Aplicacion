using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
using FluentValidation;
using SistemaDeBoleteria.Core.DTOs;
namespace SistemaDeBoleteria.Core.Validations
{
    public class SectorValidator : AbstractValidator<CrearActualizarSectorDTO>
    {
        public SectorValidator()
        {

            RuleFor(s => (int)s.Capacidad)
                .GreaterThan(0).WithMessage("La capacidad debe ser mayor a 0");
        }
    }
}