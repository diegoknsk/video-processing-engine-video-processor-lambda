using Amazon.S3;
using Amazon.S3.Model;
using VideoProcessor.Domain.Ports;

namespace VideoProcessor.Infra.S3;

/// <summary>
/// Implementação de <see cref="IS3VideoStorage"/> usando Amazon S3.
/// </summary>
public class S3VideoStorage(IAmazonS3 s3Client) : IS3VideoStorage
{
    private const int UploadConcurrency = 8;
    private const string FramesContentType = "image/jpeg";

    /// <inheritdoc />
    public async Task<string> DownloadToTempAsync(string bucket, string key, string localTempPath, CancellationToken ct = default)
    {
        var dir = Path.GetDirectoryName(localTempPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = bucket,
                Key = key
            };
            using var response = await s3Client.GetObjectAsync(request, ct).ConfigureAwait(false);
            await using var fileStream = new FileStream(localTempPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            await response.ResponseStream.CopyToAsync(fileStream, ct).ConfigureAwait(false);
            return localTempPath;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new FileNotFoundException($"Vídeo não encontrado no S3: s3://{bucket}/{key}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> UploadFramesAsync(string bucket, string prefix, IEnumerable<string> localFramePaths, CancellationToken ct = default)
    {
        var prefixNormalized = prefix.TrimEnd('/');
        if (!string.IsNullOrEmpty(prefixNormalized))
            prefixNormalized += "/";

        var paths = localFramePaths.ToList();
        var results = new string[paths.Count];
        var semaphore = new SemaphoreSlim(UploadConcurrency);

        var tasks = paths.Select((localPath, index) =>
        {
            return Task.Run(async () =>
            {
                await semaphore.WaitAsync(ct).ConfigureAwait(false);
                try
                {
                    var fileName = Path.GetFileName(localPath);
                    var key = prefixNormalized + fileName;
                    var request = new PutObjectRequest
                    {
                        BucketName = bucket,
                        Key = key,
                        FilePath = localPath,
                        ContentType = FramesContentType
                    };
                    await s3Client.PutObjectAsync(request, ct).ConfigureAwait(false);
                    results[index] = key;
                }
                finally
                {
                    semaphore.Release();
                }
            }, ct);
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);
        return results;
    }
}
