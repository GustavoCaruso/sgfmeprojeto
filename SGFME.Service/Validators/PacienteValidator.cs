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
            // Validação do nome completo
            RuleFor(p => p.nomeCompleto)
                .NotEmpty().WithMessage("Informe o Nome Completo!")
                .NotNull().WithMessage("Informe o Nome Completo!");

            // Validação do peso
            RuleFor(p => p.peso)
                .GreaterThan(0).WithMessage("O peso deve ser maior que zero!");

            // Validação da altura
            RuleFor(p => p.altura)
                .GreaterThan(0).WithMessage("A altura deve ser maior que zero!");

            // Validação da data de nascimento
            RuleFor(p => p.dataNascimento)
                .NotEmpty().WithMessage("Informe a Data de Nascimento!")
                .LessThan(DateTime.Now).WithMessage("A Data de Nascimento deve ser no passado!");

            // Validação da idade
            RuleFor(p => p.idade)
                .GreaterThan(0).WithMessage("A idade deve ser maior que zero!");

            // Validação do nome da mãe
            RuleFor(p => p.nomeMae)
                .NotEmpty().WithMessage("Informe o Nome da Mãe!")
                .NotNull().WithMessage("Informe o Nome da Mãe!");

            // Validação do RG
            RuleFor(p => p.rgNumero)
                .NotEmpty().WithMessage("Informe o Número do RG!");
            RuleFor(p => p.rgDataEmissao)
                .NotEmpty().WithMessage("Informe a Data de Emissão do RG!")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A Data de Emissão do RG deve ser hoje ou no passado!");
            RuleFor(p => p.rgOrgaoExpedidor)
                .NotEmpty().WithMessage("Informe o Órgão Expedidor do RG!");
            RuleFor(p => p.rgUfEmissao)
                .NotEmpty().WithMessage("Informe a UF de Emissão do RG!");

            // Validação do CNS
            RuleFor(p => p.cnsNumero)
                .NotEmpty().WithMessage("Informe o Número do CNS!");

            // Validação do CPF
            RuleFor(p => p.cpfNumero)
                .NotEmpty().WithMessage("Informe o Número do CPF!")
                .Matches(@"^\d{11}$").WithMessage("O CPF deve conter 11 dígitos!");

            // Validação do nome do cônjuge (opcional, pode estar vazio)
            RuleFor(p => p.nomeConjuge)
                .NotNull().WithMessage("Informe o Nome do Cônjuge!");

            // Validação da data de cadastro
            RuleFor(p => p.dataCadastro)
                .NotEmpty().WithMessage("Informe a Data de Cadastro!")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A Data de Cadastro deve ser hoje ou no passado!");

        }
    }
}
