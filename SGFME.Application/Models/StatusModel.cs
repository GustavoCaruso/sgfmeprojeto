using SGFME.Domain.Entidades;
using System.Text.Json.Serialization;

namespace SGFME.Application.Models
{
    public class StatusModel
    {
        public long id { get; set; }
        public string nome { get; set; }
        [JsonIgnore]
        public virtual ICollection<Medico> medico { get; set; } = new List<Medico>();

        [JsonIgnore]
        public virtual ICollection<Representante> representante { get; set; } = new List<Representante>();


        [JsonIgnore]
        public virtual ICollection<Paciente> paciente { get; set; } = new List<Paciente>();

        [JsonIgnore]
        public virtual ICollection<EstabelecimentoSaude> estabelecimentosaude { get; set; } = new List<EstabelecimentoSaude>();

        [JsonIgnore]
        public virtual ICollection<Cid> cid { get; set; } = new List<Cid>();

        [JsonIgnore]
        public virtual ICollection<Medicamento> medicamento { get; set; } = new List<Medicamento>();
    }
}
