namespace backend_squad1.Models;

public class Empregado
{
    public int Matricula { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Funcao { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public int Resolutor { get; set; }
    public int Setor_idSetor { get; set; }
    public int Filial_idFilial { get; set; }
}