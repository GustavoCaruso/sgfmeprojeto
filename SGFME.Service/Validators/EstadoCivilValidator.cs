using FluentValidation;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Service.Validators
{
    public class EstadoCivilValidator : AbstractValidator<EstadoCivil>
    {
        public EstadoCivilValidator()
        {
           
        }
    }
}
