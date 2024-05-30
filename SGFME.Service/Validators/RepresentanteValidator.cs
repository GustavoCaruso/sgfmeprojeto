using FluentValidation;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Service.Validators
{
    public class RepresentanteValidator : AbstractValidator<Representante>
    {
        public RepresentanteValidator()
        {
       
            RuleFor(p => p.nomeCompleto)
                .NotEmpty().WithMessage("Informe o Nome Completo!")
                .NotNull().WithMessage("Informe o Nome Completo!");

            RuleFor(p => p.dataNascimento)
                .NotEmpty().WithMessage("Informe a Data de Nascimento!")
                .LessThan(DateTime.Now).WithMessage("A Data de Nascimento deve ser no passado!");

            RuleFor(p => p.dataCadastro)
               .NotEmpty().WithMessage("Informe a Data de Cadastro!")
               .LessThanOrEqualTo(DateTime.Now).WithMessage("A Data de Cadastro deve ser hoje ou no passado!");

            RuleFor(p => p.rgNumero)
                .NotEmpty().WithMessage("Informe o Número do RG!");
            
            RuleFor(p => p.rgDataEmissao)
                .NotEmpty().WithMessage("Informe a Data de Emissão do RG!")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A Data de Emissão do RG deve ser hoje ou no passado!");
           
            RuleFor(p => p.rgOrgaoExpedidor)
                .NotEmpty().WithMessage("Informe o Órgão Expedidor do RG!");
          
            RuleFor(p => p.rgUfEmissao)
                .NotEmpty().WithMessage("Informe a UF de Emissão do RG!");

            RuleFor(p => p.cnsNumero)
                .NotEmpty().WithMessage("Informe o Número do CNS!");

            RuleFor(p => p.cpfNumero)
                .NotEmpty().WithMessage("Informe o Número do CPF!")
                .Matches(@"^\d{11}$").WithMessage("O CPF deve conter 11 dígitos!");

        }
    }
}
