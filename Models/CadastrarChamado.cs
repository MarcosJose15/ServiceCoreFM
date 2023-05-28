namespace backend_squad1.Models;

public class CadastrarChamado
{
    public string Nome { get; set; } = string.Empty;
    public string DataRelato { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Empregado_Matricula { get; set; }
    public string Tipo { get; set; } = string.Empty;
}