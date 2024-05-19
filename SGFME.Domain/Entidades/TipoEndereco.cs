using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class TipoEndereco : BaseEntity
    {
        public TipoEndereco()
        {
            this.endereco = new HashSet<Endereco>();
        }
        public string nome { get; set; }
        public virtual ICollection<Endereco> endereco { get; set; }
    }
}
