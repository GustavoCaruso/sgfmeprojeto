using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Dispensacao : BaseEntity
    {
        public long idPaciente { get; set; }
        public virtual Paciente paciente { get; set; }
        public long idCid { get; set; }
        public virtual Cid cid { get; set; }
        public DateTime inicioApac { get; set; }
        public DateTime fimApac { get; set; }
        public string observacao { get; set; }
        public List<DispensacaoMedicamento> dispensacaomedicamento { get; set; }
    }
}
