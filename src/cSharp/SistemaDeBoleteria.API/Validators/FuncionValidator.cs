using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SistemaDeBoleteria.Core;

namespace SistemaDeBoleteria.API.Validators
{
    public class FuncionValidator : AbstractValidator<Funcion>
    {
        public FuncionValidator()
        {
            RuleFor(f => f.IdEvento)
                .GreaterThan(0).WithMessage("");
            RuleFor(f => f.IdSector)
                .GreaterThan(0).WithMessage("");
            RuleFor(f => f.IdSesion)
                .GreaterThan(0).WithMessage("");
        }
    }
}