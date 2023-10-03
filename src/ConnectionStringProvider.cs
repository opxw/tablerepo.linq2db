using Sap.Data.Hana;
using MySqlConnector;
using Npgsql;
using System.Data.SqlClient;

namespace TableRepo.linq2db
{
    internal class ConnectionStringProvider
    {
        public string BuildConnectionString(IDbConnectionParameter parameter)
        {
            var result = string.Empty;
            try
            {
                switch (parameter.ProviderType)
                {
                    case DbProviderType.PostgreSql:
                        var postgreBuilder = new NpgsqlConnectionStringBuilder()
                        {
                            Host = parameter.HostName,
                            Port = parameter.Port,
                            Database = parameter.Database,
                            Username = parameter.User,
                            Password = parameter.Password
                        };

                        if (parameter.ConnectTimeOut > 0)
                            postgreBuilder.Timeout = parameter.ConnectTimeOut;
                        if (parameter.CommandTimeOut > 0)
                            postgreBuilder.CommandTimeout = parameter.CommandTimeOut;

                        result = postgreBuilder.ConnectionString;
                        break;
                    case DbProviderType.MySql:
                        var mysqlBuilder = new MySqlConnectionStringBuilder()
                        {
                            Server = parameter.HostName,
                            Port = (uint)parameter.Port,
                            Database = parameter.Database,
                            UserID = parameter.User,
                            Password = parameter.Password
                        };

                        if (parameter.ConnectTimeOut > 0)
                            mysqlBuilder.ConnectionTimeout = (uint)parameter.ConnectTimeOut;
                        if (parameter.CommandTimeOut > 0)
                            mysqlBuilder.DefaultCommandTimeout = (uint)parameter.CommandTimeOut;

                        result = mysqlBuilder.ConnectionString;
                        break;
                    case DbProviderType.MsSql:
                        var instanceName = string.IsNullOrWhiteSpace(parameter.InstanceName) ? "" : @"\" + parameter.InstanceName;
                        var mssqlBuilder = new SqlConnectionStringBuilder()
                        {
                            DataSource = parameter.HostName,
                            InitialCatalog = parameter.Database,
                            UserID = parameter.User,
                            Password = parameter.Password,
                            Encrypt = false,
                        };

                        if (parameter.ConnectTimeOut > 0)
                            mssqlBuilder.ConnectTimeout = (int)parameter.ConnectTimeOut;

                        result = mssqlBuilder.ConnectionString;
                        break;
                    case DbProviderType.SapHana:
                        var hanaBuilder = new HanaConnectionStringBuilder()
                        {
                            Server = parameter.HostName + ":" + parameter.Port.ToString(),
                            Database = parameter.Database,
                            UserName = parameter.User,
                            Password = parameter.Password,
                        };

                        if (parameter.ConnectTimeOut > 0)
                            hanaBuilder.ConnectionTimeout = (int)parameter.ConnectTimeOut;

                        result = hanaBuilder.ConnectionString;
                        break;
                }
            }
            catch (Exception ex)
            {
            }

            return result;
        }
    }
}