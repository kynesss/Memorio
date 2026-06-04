using Amazon.S3;
using Amazon.S3.Model;
using Memorio.Flashcards.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace Memorio.Flashcards.Infrastructure.Storage;

internal sealed class S3CardMediaStorage : ICardMediaStorage
{
    private readonly IAmazonS3 _client;
    private readonly CardMediaStorageOptions _options;

    public S3CardMediaStorage(IAmazonS3 client, IOptions<CardMediaStorageOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<string> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken)
    {
        await EnsureBucketAsync(cancellationToken);

        await _client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            InputStream = content,
            ContentType = contentType,
            AutoCloseStream = false
        }, cancellationToken);

        return BuildUrl(objectKey);
    }

    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken)
    {
        await _client.DeleteObjectAsync(_options.BucketName, objectKey, cancellationToken);
    }

    private async Task EnsureBucketAsync(CancellationToken cancellationToken)
    {
        if (!_options.CreateBucketIfMissing)
        {
            return;
        }

        try
        {
            await _client.GetBucketLocationAsync(_options.BucketName, cancellationToken);
        }
        catch (AmazonS3Exception exception) when (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            await _client.PutBucketAsync(_options.BucketName, cancellationToken);
        }
    }

    private string BuildUrl(string objectKey)
    {
        var baseUrl = string.IsNullOrWhiteSpace(_options.PublicBaseUrl)
            ? $"{_options.Endpoint.TrimEnd('/')}/{_options.BucketName}"
            : _options.PublicBaseUrl.TrimEnd('/');

        return $"{baseUrl}/{EscapeObjectKey(objectKey)}";
    }

    private static string EscapeObjectKey(string objectKey) =>
        string.Join('/', objectKey.Split('/').Select(Uri.EscapeDataString));
}
