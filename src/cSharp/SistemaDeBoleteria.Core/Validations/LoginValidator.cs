using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using SistemaDeBoleteria.Core.DTOs;

namespace SistemaDeBoleteria.Core.Validations
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(l => l.Email)
                .NotEmpty().WithMessage("El email no puede estar vacío.")
                .EmailAddress().WithMessage("El email no es válido");
        }
    }
    public class RegisterValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterValidator()
        {
            RuleFor(r => r.NombreUsuario)
                .NotEmpty().WithMessage("El nombre no puede estar vacío.");
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("El email no puede estar vacío.")
                .EmailAddress().WithMessage("El email no es válido");
            RuleFor(r => r.Contraseña)
                .NotEmpty().WithMessage("La contraseña no puede estar vacía.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
            RuleFor(r => r.Rol)
                .IsInEnum().WithMessage("El rol seleccionado es inválido");
        }
    }
}