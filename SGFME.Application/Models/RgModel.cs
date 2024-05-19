namespace SGFME.Application.Models
{
    public class RgModel
    {
        public long id { get; set; }
        public string numero { get; set; }
        public DateTime dataEmissao { get; set; }
        public string orgaoExpedidor { get; set; }
        public string ufEmissao { get; set; }
    }
}
