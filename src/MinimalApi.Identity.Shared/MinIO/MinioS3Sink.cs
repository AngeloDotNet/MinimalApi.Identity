using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using MinimalApi.Identity.Shared.Options;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace MinimalApi.Identity.Shared.MinIO;

public class MinioS3Sink : ILogEventSink
{
    private readonly IAmazonS3 s3Client;
    private readonly JsonFormatter jsonFormatter;
    private readonly string bucketName;
    private readonly string logObjectKey;

    public MinioS3Sink(MinioOptions options)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = options.Endpoint,
            ForcePathStyle = true
        };

        s3Client = new AmazonS3Client(options.AccessKey, options.SecretKey, config);
        bucketName = options.BucketName;
        logObjectKey = options.LogObjectKey;
        jsonFormatter = new JsonFormatter();
    }

    public void Emit(LogEvent logEvent)
    {
        // Serialize logEvent to JSON
        string logLine;
        using (var sw = new StringWriter())
        {
            jsonFormatter.Format(logEvent, sw);
            logLine = sw.ToString();
        }

        logLine += "\n";
        UploadLogAsync(logLine).Wait();
    }

    private async Task UploadLogAsync(string logLine)
    {
        var existingLogs = "";

        try
        {
            var getObjectResponse = await s3Client.GetObjectAsync(bucketName, logObjectKey);
            using var reader = new StreamReader(getObjectResponse.ResponseStream);
            existingLogs = await reader.ReadToEndAsync();
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            existingLogs = "";
        }

        var combinedLogs = existingLogs + logLine;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(combinedLogs));

        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = logObjectKey,
            InputStream = stream
        };

        await s3Client.PutObjectAsync(putRequest);
    }
}