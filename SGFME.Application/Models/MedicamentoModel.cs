using SGFME.Domain.Entidades;
using System.Text.Json.Serialization;

namespace SGFME.Application.Models
{
    public class MedicamentoModel
    {
        public long id { get; set; }
        public string nome { get; set; }
        public long idStatus { get; set; }
        public virtual Status status { get; set; }
    }
}
