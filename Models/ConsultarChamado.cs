namespace backend_squad1.Models;

public class ConsultaChamado
{
      public int idChamado { get; set; }
      public string Nome { get; set; }
      public string DataRelato { get; set; }
      public string Descricao { get; set; }
      public string Prioridade { get; set; }
      public string HorarioAbertura { get; set; }
      public string HorarioUltimaAtualizacao { get; set; }
      public string Status { get; set; }
      public string TempoDecorrido { get; set; }
      public int Empregado_Matricula { get; set; }
      public string Tipo { get; set; }
      public List<LinkMidia> LinkMidia { get; set; }
}