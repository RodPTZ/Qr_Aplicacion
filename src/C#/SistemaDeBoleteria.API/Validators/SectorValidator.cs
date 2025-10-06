using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core;
using FluentValidation;
namespace SistemaDeBoleteria.API.Validators
{
    public class SectorValidator : AbstractValidator<Sector>
    {
        public SectorValidator()
        {
            RuleFor(s => s.TipoSector)
                .MinimumLength(5).WithMessage("");
            RuleFor(s => s.IdLocal)
                .GreaterThan(0).WithMessage("");
        }
    }
}