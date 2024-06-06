using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Cid : BaseEntity
    {
        public string codigo { get; set; }
        public string descricao { get; set; }

        public long idStatus { get; set; }
        public virtual Status status { get; set; }

        public long idVersaoCid { get; set; }
        public virtual VersaoCid versaocid { get; set; }
    }
}
