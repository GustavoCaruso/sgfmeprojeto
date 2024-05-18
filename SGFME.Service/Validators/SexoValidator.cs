using FluentValidation;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Service.Validators
{
    public class SexoValidator : AbstractValidator<Sexo>
    {
        public SexoValidator()
        {
            //descricao nao pode ser vazia
            RuleFor(p => p.nome).NotEmpty().WithMessage("Informe um nome!");
            //descricao nao pode ser null
            RuleFor(p => p.nome).NotNull().WithMessage("Informe um nome!");
        }
    }
}
