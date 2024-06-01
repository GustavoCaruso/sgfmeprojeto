using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class EstadoCivil : BaseEntity
    {
        public string nome { get; set; }

        //Relação com Paciente
        public virtual ICollection<Paciente> paciente { get; set; } = new List<Paciente>();
    }
}
