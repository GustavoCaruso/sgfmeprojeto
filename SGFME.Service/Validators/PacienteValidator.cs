using FluentValidation;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Service.Validators
{
    public class PacienteValidator : AbstractValidator<Paciente>
    {
        public PacienteValidator() 
        {
            RuleFor(p => p.nomeCompleto).NotEmpty().WithMessage("Informe o Nome Completo!");
            RuleFor(p => p.nomeCompleto).NotNull().WithMessage("Informe o nome completo!");
        }
    }
}
