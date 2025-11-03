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
            RuleFor(f => f.IdEvento)
                .GreaterThan(0).WithMessage("El IdSesion debe ser mayor que 0");
            RuleFor(f => f.IdSector)
                .GreaterThan(0).WithMessage("El IdSector debe ser mayor que 0");
            RuleFor(f => f.Apertura)
                .NotEmpty().WithMessage("La apertura no puede estar vacía")
                .GreaterThanOrEqualTo(DateTime.Now).WithMessage("La fecha no puede ser anterior a hoy");
            RuleFor(f => f.Cierre)
                .NotEmpty().WithMessage("El cierre no puede estar vacío")
                .GreaterThan(f => f.Apertura).WithMessage("El cierre no puede ser antes de la apertura");
        }
    }
    public class ActualizarFuncionValidator : AbstractValidator<ActualizarFuncionDTO>
    {
        public ActualizarFuncionValidator()
        {
            RuleFor(f => f.IdSector)
                .GreaterThan(0).WithMessage("El IdSector debe ser mayor que 0");
            RuleFor(f => f.Apertura)
                .NotEmpty().WithMessage("La apertura no puede estar vacía")
                .GreaterThanOrEqualTo(DateTime.Now).WithMessage("La apertura debe ser posterior o igual a la fecha actual");
            RuleFor(f => f.Cierre)
                .NotEmpty().WithMessage("El cierre no puede estar vacío")
                .GreaterThan(f => f.Apertura).WithMessage("El cierre no puede ser antes de la apertura");
        }
    }
}