using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class CorRaca : BaseEntity
    {
        public string nome { get; set; }

        public virtual ICollection<Medico> medico { get; set; } = new List<Medico>();
        //Relação com Paciente
        public virtual ICollection<Paciente> paciente { get; set; } = new List<Paciente>();
    }
}
