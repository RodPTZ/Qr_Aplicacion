using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SistemaDeBoleteria.Core;

namespace SistemaDeBoleteria.API.Validators
{
    public class OrdenValidator : AbstractValidator<Orden>
    {
        public OrdenValidator()
        {
            RuleFor(o => o.IdSesion)
                .GreaterThan(0).WithMessage("");
            RuleFor(o => o.MedioDePago)
                .MinimumLength(2).WithMessage("");
            RuleFor(o => o.IdCliente)
                .GreaterThan(0).WithMessage("");
        }
    }
}