namespace MinimalApi.Identity.Shared.Options;

public class MinioOptions
{
    public string Endpoint { get; set; } = null!; // MinIO server endpoint: http://127.0.0.1:9000
    public string AccessKey { get; set; } = null!; // access credentials
    public string SecretKey { get; set; } = null!; // access credentials
    public string BucketName { get; set; } = null!; // bucket name: logs
    public string LogObjectKey { get; set; } = null!; // object prefix: serilog-demo.json
}