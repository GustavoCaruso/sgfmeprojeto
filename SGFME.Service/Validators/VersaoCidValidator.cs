using FluentValidation;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Service.Validators
{
    public class VersaoCidValidator : AbstractValidator<VersaoCid>
    {
        public VersaoCidValidator()
        {
            RuleFor(p => p.nome).NotEmpty().WithMessage("Informe o Nome!");
            RuleFor(p => p.nome).NotNull().WithMessage("Informe o Nome!");
        }

    }
}
