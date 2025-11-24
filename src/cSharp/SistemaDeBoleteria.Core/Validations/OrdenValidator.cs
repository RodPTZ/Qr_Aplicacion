using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Validations
{
    public class OrdenValidator : AbstractValidator<CrearOrdenDTO>
    {
        public OrdenValidator()
        {
            RuleFor(o => o.IdTarifa)
                .GreaterThan(0).WithMessage("El IdCliente debe ser mayor a 0");
            RuleFor(o => o.IdCliente)
                .GreaterThan(0).WithMessage("El IdSesion debe ser mayor a 0");
            RuleFor(o => o.MedioDePago)
                .IsInEnum().WithMessage("Medio de pago fuera del rango");
        }
    }
}