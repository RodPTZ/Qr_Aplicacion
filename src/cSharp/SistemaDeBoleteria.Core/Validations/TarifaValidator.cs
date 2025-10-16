using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace SistemaDeBoleteria.Core.Validations
{
    public class TarifaValidator : AbstractValidator<Tarifa>
    {
        public TarifaValidator()
        {
            RuleFor(t => t.IdFuncion)
                .GreaterThan(0).WithMessage("");
            RuleFor(t => t.estado)
                .MinimumLength(5).WithMessage("");
            RuleFor(t => t.precio)
                .GreaterThan(0).WithMessage("");
            RuleFor(t => (int)t.stock)
                .GreaterThan(10).WithMessage("");
        }
    }
}