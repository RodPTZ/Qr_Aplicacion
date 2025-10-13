using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SistemaDeBoleteria.Core;

namespace SistemaDeBoleteria.API.Validators
{
    public class ClienteValidator : AbstractValidator<Cliente>
    {
        public ClienteValidator()
        {
            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("")
                .MaximumLength(60).WithMessage("");
            RuleFor(c => c.Apellido)
                .NotEmpty().WithMessage("")
                .MaximumLength(60).WithMessage("");
            RuleFor(c => c.Localidad)
                .NotEmpty().WithMessage("")
                .MaximumLength(60).WithMessage("");
            RuleFor(c => c.DNI.ToString())
                .Length(8).WithMessage("");
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("")
                .EmailAddress().WithMessage("");
            RuleFor(c => c.Telefono.ToString())
                .Length(10).WithMessage("");
            RuleFor(c => c.Edad)
                .LessThan(130).WithMessage("");
        }
    }
}