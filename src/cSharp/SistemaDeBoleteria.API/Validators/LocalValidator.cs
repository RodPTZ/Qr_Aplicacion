using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SistemaDeBoleteria.Core;

namespace SistemaDeBoleteria.API.Validators
{
    public class LocalValidator : AbstractValidator<Local>
    {
        public LocalValidator()
        {
            RuleFor(l => l.Ubicacion)
                .MinimumLength(4).WithMessage("la ubicación debe más de 3 caracteres.");
        }
    }
}