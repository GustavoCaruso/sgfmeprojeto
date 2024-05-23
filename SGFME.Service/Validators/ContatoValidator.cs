using FluentValidation;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Service.Validators
{
    public class ContatoValidator : AbstractValidator<Contato>
    {
        public ContatoValidator()
        {
            //descricao nao pode ser vazia
            RuleFor(p => p.valor).NotEmpty().WithMessage("Informe o valor!");
            //descricao nao pode ser null
            RuleFor(p => p.valor).NotNull().WithMessage("Informe o valor!");
        }
    }
}
