using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Status : BaseEntity
    {
        public string nome { get; set; }
        public virtual ICollection<Representante> representante { get; set; } = new List<Representante>();



        //Relação com Paciente
        public virtual ICollection<Paciente> paciente { get; set; } = new List<Paciente>();
        public virtual ICollection<Medico> medico { get; set; } = new List<Medico>();
        public virtual ICollection<EstabelecimentoSaude> estabelecimentosaude { get; set; } = new List<EstabelecimentoSaude>();
    }
}
