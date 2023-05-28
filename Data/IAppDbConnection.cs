namespace backend_squad1.Data;

public interface IAppDbConnection
{
    void Open();
    void Close();

    void ExecuteNonQuery(AppDbModels.ExecuteCommand values);
    T? ExecuteScalar<T>(AppDbModels.ExecuteScalar values);
    List<T> ExecuteReader<T>(AppDbModels.ExecuteReader values);

    void Commit();
    void Rollback();
}
