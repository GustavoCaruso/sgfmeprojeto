using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Medicamento : BaseEntity
    {
        public string nome { get; set; }
        public long idStatus { get; set; }
        public virtual Status status { get; set; }
    }
}
