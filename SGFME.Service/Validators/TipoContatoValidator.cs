using FluentValidation;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Service.Validators
{
    public class TipoContatoValidator : AbstractValidator<TipoContato>
    {
        public TipoContatoValidator()
        {
            RuleFor(p => p.nome).NotEmpty().WithMessage("Informe o Nome!");
            RuleFor(p => p.nome).NotNull().WithMessage("Informe o nome!");
        }
    }
}
