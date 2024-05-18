using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class TipoContato : BaseEntity
    {
        public TipoContato()
        {
            this.contato = new HashSet<Contato>();
        }
        public String nome { get; set; }
        public virtual ICollection<Contato> contato { get; set; }
    }
}
