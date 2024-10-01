namespace SGFME.Domain.Entidades;

public class Dispensacao : BaseEntity
{
    public long idPaciente { get; set; }
    public virtual Paciente paciente { get; set; }
    public long idCid { get; set; }
    public virtual Cid cid { get; set; }
    public DateTime inicioApac { get; set; }
    public DateTime fimApac { get; set; }
    public string? observacao { get; set; }

    // Data de Renovação e Suspensão
    public DateTime? dataRenovacao { get; set; } // Data de renovação do processo
    public DateTime? dataSuspensao { get; set; } // Data de suspensão após 3 meses sem retirada

    // Relacionamento com StatusProcesso e TipoProcesso
    public long idStatusProcesso { get; set; }
    public virtual StatusProcesso statusprocesso { get; set; }

    public long idTipoProcesso { get; set; }
    public virtual TipoProcesso tipoprocesso { get; set; }

    public virtual ICollection<DispensacaoMedicamento> medicamento { get; set; }

}
