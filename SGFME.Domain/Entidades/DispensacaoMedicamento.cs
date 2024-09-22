using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class DispensacaoMedicamento : BaseEntity
    {
        public long idMedicamento { get; set; }
        public virtual Medicamento medicamento { get; set; }
        public int quantidade { get; set; }
        public bool recibo { get; set; }
        public bool recebido { get; set; }
        public long idDispensacao { get; set; }
    }
}
