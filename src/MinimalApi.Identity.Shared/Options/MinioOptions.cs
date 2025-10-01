namespace MinimalApi.Identity.Shared.Options;

public class MinioOptions
{
    public string Endpoint { get; set; } = null!; // endpoint del server MinIO: http://127.0.0.1:9000
    public string AccessKey { get; set; } = null!; // credenziali di accesso
    public string SecretKey { get; set; } = null!; // credenziali di accesso
    public string BucketName { get; set; } = null!; // nome del bucket: logs
    public string LogObjectKey { get; set; } = null!; // prefisso per gli oggetti: serilog-demo.json
}