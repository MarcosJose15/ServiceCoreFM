namespace backend_squad1.Data;

public static class AppDbModels
{
    public class AppDbParameter
    {
        public string Column { get; set; } = string.Empty;
        public object? Value { get; set; }
    }

    public class AppDbParameters
    {
        private List<AppDbParameter> parameters = new List<AppDbParameter>();

        public List<AppDbParameter> Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        private AppDbParameters() { }


        public AppDbParameters add(String column, object? value)
        {
            this.parameters.Add(new AppDbParameter { Column = column, Value = value });
            return this;
        }

        public static AppDbParameters GetInstance()
        {
            return new AppDbParameters();
        }
    }

    public class ExecuteQuery {
        public string QuerySql { get; set; } = string.Empty;
    }

    public class ExecuteCommand: ExecuteQuery {
        public AppDbParameters parameters { get; set; } = AppDbParameters.GetInstance();
    }

    public class ExecuteScalar: ExecuteQuery {
        public AppDbParameters parameters { get; set; } = AppDbParameters.GetInstance();
    }

    public class ExecuteReader: ExecuteQuery {
        public AppDbParameters parameters { get; set; } = AppDbParameters.GetInstance();
    }

    public class AppDbSqlException: Exception {
        public int code;

        private AppDbSqlException(int code, string? message, Exception? innerException) : base(message, innerException) {
            this.code = code;
        }

        public static AppDbSqlException GetInstance(int code, string? message, Exception? innerException) {
            return new AppDbSqlException(code, message, innerException);
        }

        public static AppDbSqlException GetInstance(int code, string? message) {
            return GetInstance(code, message, null);            
        }

        public static AppDbSqlException GetInstance(int code) {
            return GetInstance(code, null);
        }
    }
}