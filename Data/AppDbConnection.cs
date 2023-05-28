using System.Data;
using MySql.Data.MySqlClient;
using Dapper;

namespace backend_squad1.Data;

public class AppDbConnection: IAppDbConnection
{
    private String ConnectionString = string.Empty;
    private IDbConnection? Connection;
    private IDbTransaction? Transaction;

    public AppDbConnection(IConfiguration configuration)
    {
        this.ConnectionString = configuration.GetConnectionString("DatabaseConnection");
    }

    private void CreateConnection()
    {
        this.Connection = new MySqlConnection(this.ConnectionString);
    }

    public void Open()
    {
        if (this.Connection is null)
            this.CreateConnection();

        this.Connection?.Open();
        this.Transaction = this.Connection?.BeginTransaction();
    }

    public void Close()
    {
        if (this.Connection is null) return;
        this.Connection.Close();

        this.Transaction = null;
        this.Connection = null;
    }

    public void Commit() {
        if (this.Connection is null) 
            throw new Exception("[ERRO] Commit: connection is possible closed");
        this.Transaction?.Commit();
    }

    public void Rollback() {
        if (this.Connection is null) 
            throw new Exception("[ERRO] Commit: connection is possible closed");
        this.Transaction?.Rollback();
    }

    private DynamicParameters prepareCommand(AppDbModels.AppDbParameters collection)
    {
        DynamicParameters parameters = new DynamicParameters();

        foreach (AppDbModels.AppDbParameter param in collection.Parameters)
            parameters.Add(param.Column, param.Value);

        return parameters;
    }

    public void ExecuteNonQuery(AppDbModels.ExecuteCommand values)
    {
        DynamicParameters parameters = this.prepareCommand(values.parameters);
        this.Connection.Execute(values.QuerySql, parameters);
    }

    public T? ExecuteScalar<T>(AppDbModels.ExecuteScalar values)
    {
        DynamicParameters parameters = this.prepareCommand(values.parameters);
        this.Connection.Execute(values.QuerySql, parameters);
        return this.Connection.ExecuteScalar<T>($"SELECT LAST_INSERT_ID()");
    }

    public List<T> ExecuteReader<T>(AppDbModels.ExecuteReader values)
    {
        DynamicParameters parameters = this.prepareCommand(values.parameters);
        return this.Connection.Query<T>(values.QuerySql, parameters).ToList();
    }
}
