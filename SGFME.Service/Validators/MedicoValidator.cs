using FluentValidation;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Service.Validators
{
    public class MedicoValidator : AbstractValidator<Medico>
    {
        public MedicoValidator() {
            RuleFor(p => p.nomeCompleto).NotEmpty().WithMessage("Informe o nome completo!");
            RuleFor(p => p.nomeCompleto).NotNull().WithMessage("Informe o nome completo!");

            RuleFor(p => p.dataNascimento).NotEmpty().WithMessage("Informe a data de nascimento!");
            RuleFor(p => p.dataNascimento).NotNull().WithMessage("Informe a data de nascimento!");

            RuleFor(p => p.crm).NotEmpty().WithMessage("Informe o CRM!");
            RuleFor(p => p.crm).NotNull().WithMessage("Informe o CRM!!");

          
        }
    }
}
