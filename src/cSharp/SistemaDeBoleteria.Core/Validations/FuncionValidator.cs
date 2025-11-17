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
            RuleFor(f => f.Fecha)
                .NotEmpty().WithMessage("La fecha no puede estar vacía")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now.ToLocalTime())).WithMessage("La fecha no puede ser anterior a hoy");
            RuleFor(f => f)
                .Must(f =>
                {
                    var ahora = TimeOnly.FromDateTime(DateTime.Now.ToLocalTime());
                    if (f.Fecha == DateOnly.FromDateTime(DateTime.Now.ToLocalTime()))
                        return f.AperturaTime >= ahora;
                    return true;
                }).WithMessage("La apertura debe ser posterior o igual a la hora actual si la fecha es hoy");
            RuleFor(f => f)
                .Must(f => f.Fecha.ToDateTime(f.CierreTime) > f.Fecha.ToDateTime(f.AperturaTime)).WithMessage("El cierre no puede ser antes de la apertura");
        }
    }
    public class ActualizarFuncionValidator : AbstractValidator<ActualizarFuncionDTO>
    {
        public ActualizarFuncionValidator()
        {
            RuleFor(f => f.IdSector)
                .GreaterThan(0).WithMessage("El IdSector debe ser mayor que 0");
           RuleFor(f => f.Fecha)
                .NotEmpty().WithMessage("La fecha no puede estar vacía")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now.ToLocalTime())).WithMessage("La fecha no puede ser anterior a hoy");
            RuleFor(f => f)
                .Must(f =>
                {
                    var ahora = TimeOnly.FromDateTime(DateTime.Now.ToLocalTime());
                    if (f.Fecha == DateOnly.FromDateTime(DateTime.Now.ToLocalTime()))
                        return f.AperturaTime >= ahora;
                    return true;
                }).WithMessage("La apertura debe ser posterior o igual a la hora actual si la fecha es hoy");
            RuleFor(f => f)
                .Must(f => f.Fecha.ToDateTime(f.CierreTime) > f.Fecha.ToDateTime(f.AperturaTime)).WithMessage("El cierre no puede ser antes de la apertura");
        }
    }
}