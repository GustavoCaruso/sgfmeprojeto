using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class EstabelecimentoSaude : BaseEntity
    {
        public String nomeFantasia { get; set; }
        public String razaoSocial { get; set; }
        public String cnes { get; set; }
        public DateTime dataCadastro { get; set; }
        public long idStatus { get; set; }
        public virtual Status status { get; set; }
        public virtual ICollection<Contato> contato { get; set; } = new List<Contato>();
        public virtual ICollection<Endereco> endereco { get; set; } = new List<Endereco>();
    }
}
