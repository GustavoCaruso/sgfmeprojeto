using SGFME.Domain.Entidades;

namespace SGFME.Application.Models
{
    public class DispensacaoModel
    {
        public long id { get; set; }
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
