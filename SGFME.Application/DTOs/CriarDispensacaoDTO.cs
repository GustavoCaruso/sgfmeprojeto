namespace SGFME.Application.DTOs;

public class CriarDispensacaoDTO
{
    public long IdPaciente { get; set; }
    public long IdCid { get; set; }
    public DateTime InicioApac { get; set; }
    public string Observacao { get; set; }
    public List<CriarDispensacaoMedicamentoDTO> Medicamentos { get; set; }
}
