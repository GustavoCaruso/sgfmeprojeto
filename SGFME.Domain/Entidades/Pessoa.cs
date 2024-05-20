using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Pessoa : BaseEntity
    {
        public string nomeCompleto { get; set; }
        public DateTime dataNascimento { get; set; }

        public string nomeMae { get; set; }

        // Calculando a idade com base na data de nascimento
        public int idade { get; set; }

    }
}
