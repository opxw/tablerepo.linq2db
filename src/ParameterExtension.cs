namespace TableRepo.linq2db
{
    internal static class ParameterExtension
    {
        public static string GetProviderName(this IDbConnectionParameter s)
        {
            string result = s.ProviderType switch
            {
                DbProviderType.MySql => "MySqlConnector",
                DbProviderType.PostgreSql => "PostgreSql",
                DbProviderType.MsSql => "SqlServer",
                DbProviderType.SapHana => "SapHana",
                _ => ""
            };

            return result;
        }

        public static string GetConnectionString(this IDbConnectionParameter s)
        {
            var provider = new ConnectionStringProvider();

            return provider.BuildConnectionString(s);
        }
    }
}