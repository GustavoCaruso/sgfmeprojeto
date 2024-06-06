using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class VersaoCid : BaseEntity
    {
        public string nome { get; set; }
        public virtual ICollection<Cid> cid { get; set; } = new List<Cid>();
    }
}
