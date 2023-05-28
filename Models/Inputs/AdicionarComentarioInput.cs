namespace backend_squad1.Models.Inputs;

public class AdicionarComentarioInput
{
    public int idChamado { get; set; }
    public int matricula { get; set; }
    public string texto { get; set; } = string.Empty;
}