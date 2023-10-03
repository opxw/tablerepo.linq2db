namespace TableRepo.linq2db
{
    public interface IDbConnectionParameter
    {
        string HostName { get; set; }
        int Port { get; set; }
        string User { get; set; }
        string Password { get; set; }
        string Database { get; set; }
        string InstanceName { get; set; }
        int ConnectTimeOut { get; set; }
        int CommandTimeOut { get; set; }
        DbProviderType ProviderType { get; set; }
    }
}