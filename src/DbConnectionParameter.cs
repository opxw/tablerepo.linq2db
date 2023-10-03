namespace TableRepo.linq2db
{
    public partial class DbConnectionParameter : IDbConnectionParameter
    {
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; }
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string InstanceName { get; set; } = string.Empty;
        public int ConnectTimeOut { get; set; } = 0;
        public int CommandTimeOut { get; set; } = 0;
        public DbProviderType ProviderType { get; set; }
    }
}