using SGFME.Domain.Entidades;
using System.Text.Json.Serialization;

namespace SGFME.Application.Models
{
    public class VersaoCidModel
    {
        public long id { get; set; }
        public string nome { get; set; }

        [JsonIgnore]
        public virtual ICollection<Cid> cid { get; set; } = new List<Cid>();


    }
}
