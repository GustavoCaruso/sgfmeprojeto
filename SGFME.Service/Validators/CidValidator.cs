using FluentValidation;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Service.Validators
{
    public class CidValidator : AbstractValidator<Cid>
    {
        public CidValidator()
        {
            RuleFor(p => p.codigo).NotEmpty().WithMessage("Informe o Código!");
            RuleFor(p => p.codigo).NotNull().WithMessage("Informe o Código!");
            RuleFor(p => p.descricao).NotEmpty().WithMessage("Informe a Descrição!");
            RuleFor(p => p.descricao).NotNull().WithMessage("Informe a Descrição!");
        }
    }
}
