namespace Memorio.Flashcards.Application.Abstractions;

public interface ICardMediaStorage
{
    Task<string> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken);

    Task DeleteAsync(string objectKey, CancellationToken cancellationToken);
}
