using SGFME.Domain.Entidades;

namespace SGFME.Application.Models
{
    public class DispensacaoMedicamentoModel
    {
        public long id { get; set; }
        public long idMedicamento { get; set; }
        public virtual Medicamento medicamento { get; set; }
        public int quantidade { get; set; }
        public bool recibo { get; set; } // Recibo para medicamentos de tarja preta
        public bool receita { get; set; } // Receita para medicamentos normais
        public bool medicamentoChegou { get; set; } // Indica se o medicamento chegou à farmácia
        public bool medicamentoEntregue { get; set; } // Indica se o medicamento foi entregue ao paciente
        public DateTime? dataEntrega { get; set; } // Data de entrega do medicamento ao paciente
        public long idDispensacao { get; set; }
    }
}
