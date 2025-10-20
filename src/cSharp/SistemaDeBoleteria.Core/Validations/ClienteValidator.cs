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
    public class ClienteValidator : AbstractValidator<CrearClienteDTO>
    {
        public ClienteValidator()
        {
            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre no puede estar vacío.")
                .MaximumLength(60).WithMessage("El nombre no puede tener más de 60 caracteres.");
            RuleFor(c => c.Apellido)
                .NotEmpty().WithMessage("El apellido no puede estar vacío.")
                .MaximumLength(60).WithMessage("El apellido no puede tener más de 60 caracteres.");
            RuleFor(c => c.Localidad)
                .NotEmpty().WithMessage("La localidad no puede estar vacía.")
                .MaximumLength(60).WithMessage("La localidad no puede tener más de 60 caracteres.");
            RuleFor(c => c.DNI.ToString())
                .Length(8).WithMessage("El DNI debe tener 8 dígitos.");
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("El email no puede estar vacío.")
                .EmailAddress().WithMessage("El email no es válido.");
            RuleFor(c => c.Telefono.ToString())
                .Length(10).WithMessage("El teléfono debe tener 10 dígitos.");
            RuleFor(c => c.Edad)
                .LessThan(130).WithMessage("La edad no puede ser mayor a 130 años.");
            RuleFor(c => c.Contraseña)
                .NotEmpty().WithMessage("La contraseña no puede estar vacía.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
        }
    }
    public class ActualizarClienteDTOValidator : AbstractValidator<ActualizarClienteDTO>
    {
        public ActualizarClienteDTOValidator()
        {
            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre no puede estar vacío.")
                .MaximumLength(60).WithMessage("El nombre no puede tener más de 60 caracteres.");
            RuleFor(c => c.Apellido)
                .NotEmpty().WithMessage("El apellido no puede estar vacío.")
                .MaximumLength(60).WithMessage("El apellido no puede tener más de 60 caracteres.");
            RuleFor(c => c.Localidad)
                .NotEmpty().WithMessage("La localidad no puede estar vacía.")
                .MaximumLength(60).WithMessage("La localidad no puede tener más de 60 caracteres.");
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("El email no puede estar vacío.")
                .EmailAddress().WithMessage("El email no es válido.");
            RuleFor(c => c.Telefono.ToString())
                .Length(10).WithMessage("El teléfono debe tener 10 dígitos.");
        }
    }
}