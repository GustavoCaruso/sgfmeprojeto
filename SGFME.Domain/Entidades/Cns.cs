using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Cns : BaseEntity
    {
        public string numero { get; set; }


        public virtual Paciente paciente { get; set; } // Navegação de volta para Paciente
    }
}
