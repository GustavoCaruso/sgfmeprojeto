using SGFME.Domain.Entidades;

namespace SGFME.Application.DTOs
{
    public class DispensacaoDTO
    {
        public long id { get; set; }
        public long idPaciente { get; set; }
        public long idCid { get; set; }
        public DateTime inicioApac { get; set; }
        public DateTime fimApac { get; set; }
        public string? observacao { get; set; }
        // Data de Renovação e Suspensão
        public DateTime? dataRenovacao { get; set; } // Data de renovação do processo
        public DateTime? dataSuspensao { get; set; } // Data de suspensão após 3 meses sem retirada
        // Relacionamento com StatusProcesso e TipoProcesso
        public long idStatusProcesso { get; set; }
        public long idTipoProcesso { get; set; }
        public List<DispensacaoMedicamentoDTO> medicamento { get; set; }
    }
}
