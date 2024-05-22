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
        //relacionamento de one to many
        public virtual ICollection<Contato> contato { get; set; } = new List<Contato>();
    }
}
