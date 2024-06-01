using SGFME.Domain.Entidades;
using System.Text.Json.Serialization;

namespace SGFME.Application.Models
{
    public class EstadoCivilModel
    {
        public long id { get; set; }
        public string nome { get; set; }

        [JsonIgnore]
        public virtual ICollection<Paciente> paciente { get; set; } = new List<Paciente>();
    }
}
