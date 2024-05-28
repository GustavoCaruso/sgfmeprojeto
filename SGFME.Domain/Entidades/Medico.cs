using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Medico : BaseEntity
    {
        public String nomeCompleto { get; set; }
        public DateTime dataNascimento { get; set; }
        public String crm { get; set; }
        public virtual ICollection<Contato> contato { get; set; } = new List<Contato>();
       
    }
}
