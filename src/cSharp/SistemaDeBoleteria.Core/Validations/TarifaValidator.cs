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
    public class TarifaValidator : AbstractValidator<CrearTarifaDTO>
    {
        public TarifaValidator()
        {
            RuleFor(t => t.IdFuncion)
                .GreaterThan(0).WithMessage("La IdFuncion debe ser mayor a 0");
            RuleFor(t => t.entrada)
                .IsInEnum().WithMessage("Debe pertenecer en el rango de opciones");
            RuleFor(t => t.precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");
            RuleFor(t => (int)t.stock)
                .GreaterThan(0).WithMessage("El stock debe ser mayor a 0");
        }
    }
    public class ActualizarTarifaDTOValidator : AbstractValidator<ActualizarTarifaDTO>
    {
        public ActualizarTarifaDTOValidator()
        {
            RuleFor(t => t.precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");
            RuleFor(t => (int)t.stock)
            .GreaterThan(0).WithMessage("El stock debe ser mayor a 0");
            RuleFor(t => t.estado)
            .IsInEnum().WithMessage("El estado debe pertenecer en el rango de opciones");
        }
    }
}