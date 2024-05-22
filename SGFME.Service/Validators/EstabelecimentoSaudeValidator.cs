using FluentValidation;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Service.Validators
{
    public class EstabelecimentoSaudeValidator : AbstractValidator<EstabelecimentoSaude>
    {
        public EstabelecimentoSaudeValidator()
        {
            
            RuleFor(p => p.nomeFantasia).NotEmpty().WithMessage("Informe o Nome Fantasia!");
            RuleFor(p => p.nomeFantasia).NotNull().WithMessage("Informe o Nome Fantasia!");

            RuleFor(p => p.razaoSocial).NotEmpty().WithMessage("Informe a Razão Social!");
            RuleFor(p => p.razaoSocial).NotNull().WithMessage("Informe a Razão Social!");

            RuleFor(p => p.cnes).NotEmpty().WithMessage("Informe o CNES!");
            RuleFor(p => p.cnes).NotNull().WithMessage("Informe o CNES!!");
        }
    }
}
