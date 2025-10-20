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
    public class FuncionValidator : AbstractValidator<CrearFuncionDTO>
    {
        public FuncionValidator()
        {
            RuleFor(f => f.IdSector)
                .GreaterThan(0).WithMessage("El IdSector debe ser mayor que 0");
            RuleFor(f => f.IdSesion)
                .GreaterThan(0).WithMessage("El IdSesion debe ser mayor que 0");
            RuleFor(f => f.Duracion)
                .NotEmpty().WithMessage("La duración no puede estar vacía");
            RuleFor(f => f.Fecha)
                .GreaterThan(DateTime.Now).WithMessage("La fecha debe ser futura");
        }
    }
    public class ActualizarFuncionValidator : AbstractValidator<ActualizarFuncionDTO>
    {
        public ActualizarFuncionValidator()
        {
            RuleFor(f => f.IdSector)
                .GreaterThan(0).WithMessage("El IdSector debe ser mayor que 0");
            RuleFor(f => f.IdSesion)
                .GreaterThan(0).WithMessage("El IdSesion debe ser mayor que 0");
            RuleFor(f => f.Duracion)
                .NotEmpty().WithMessage("La duración no puede estar vacía");
            RuleFor(f => f.Fecha)
                .GreaterThan(DateTime.Now).WithMessage("La fecha debe ser futura");
        }
    }
}