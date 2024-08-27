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
        public virtual ICollection<Funcionario> funcionario { get; set; } = new List<Funcionario>();
        public virtual ICollection<Usuario> usuario { get; set; } = new List<Usuario>();


        public virtual ICollection<Cid> cid { get; set; } = new List<Cid>();
        public virtual ICollection<Medicamento> medicamento { get; set; } = new List<Medicamento>();
    }
}
