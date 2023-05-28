namespace backend_squad1.Constants.Sql;

public static class ChamadoSql
{
    public static string CreateComentarioSql = @"
INSERT INTO Comentario 
    (idChamado, matricula, texto) 
VALUES 
    (@idChamado, @matricula, @Texto)
";
}