using SGFME.Domain.Entidades;
using System.ComponentModel.DataAnnotations;

namespace SGFME.Application.DTOs;

public class AlterarDispensacaoDTO
{
    [Required]
    public long Id { get; set; }
    [Required]
    public long IdPaciente { get; set; }
    [Required]
    public long IdCid { get; set; }
    [Required]
    public DateTime InicioApac { get; set; }
    public string? Observacao { get; set; }
    [Required]
    [MinLength(1, ErrorMessage = "Deve haver pelo menos um medicamento")]
    public List<AlterarDispensacaoMedicamentoDTO> Medicamentos { get; set; }

    public ICollection<DispensacaoMedicamento> ToEntity(ICollection<AlterarDispensacaoMedicamentoDTO> medicamentos)
    {
        return medicamentos.Select(x => new DispensacaoMedicamento
        {
            id = x.Id,
            IdMedicamento = x.idMedicamento,
            quantidade = x.quantidade,
            recibo = x.recibo,
            recebido = x.recebido
        }).ToList();
    }
}
