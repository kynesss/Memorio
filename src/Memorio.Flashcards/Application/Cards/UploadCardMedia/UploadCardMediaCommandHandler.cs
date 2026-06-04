using System.Text.RegularExpressions;
using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Abstractions;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Application.Mapping;
using Memorio.Flashcards.Domain;
using Memorio.Flashcards.Infrastructure.Persistence;

namespace Memorio.Flashcards.Application.Cards.UploadCardMedia;

public sealed partial class UploadCardMediaCommandHandler : IRequestHandler<UploadCardMediaCommand, ErrorOr<CardMediaDto>>
{
    private const long MaxFileSize = 10 * 1024 * 1024;

    private static readonly IReadOnlyDictionary<string, string> ContentTypes = new Dictionary<string, string>
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".gif"] = "image/gif",
        [".webp"] = "image/webp"
    };

    private readonly FlashcardsDbContext _dbContext;
    private readonly ICardMediaStorage _storage;
    private readonly TimeProvider _clock;

    public UploadCardMediaCommandHandler(
        FlashcardsDbContext dbContext,
        ICardMediaStorage storage,
        TimeProvider clock)
    {
        _dbContext = dbContext;
        _storage = storage;
        _clock = clock;
    }

    public async Task<ErrorOr<CardMediaDto>> Handle(UploadCardMediaCommand command, CancellationToken cancellationToken)
    {
        if (command.File.Length == 0)
        {
            return FlashcardsErrors.CardMediaFileMissing;
        }

        if (command.File.Length > MaxFileSize)
        {
            return FlashcardsErrors.CardMediaFileTooLarge;
        }

        var extension = Path.GetExtension(command.File.FileName).ToLowerInvariant();
        if (!ContentTypes.TryGetValue(extension, out var contentType))
        {
            return FlashcardsErrors.CardMediaUnsupportedType;
        }

        await using var stream = command.File.OpenReadStream();
        if (!await HasValidSignatureAsync(stream, extension, cancellationToken))
        {
            return FlashcardsErrors.CardMediaUnsupportedType;
        }

        var card = await _dbContext.FindOwnedCardAsync(command.UserId, command.CardId, cancellationToken);
        if (card is null)
        {
            return FlashcardsErrors.CardNotFound;
        }

        stream.Position = 0;

        var fileName = BuildFileName(command.File.FileName, extension);
        var objectKey = $"users/{command.UserId}/cards/{command.CardId}/{fileName}";
        var url = await _storage.UploadAsync(objectKey, stream, contentType, cancellationToken);
        var media = CardMediaItem.Create(command.CardId, url, objectKey, fileName, command.File.Length, _clock);

        _dbContext.CardMediaItems.Add(media);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await _storage.DeleteAsync(objectKey, CancellationToken.None);
            throw;
        }

        return media.ToDto();
    }

    private static string BuildFileName(string fileName, string extension)
    {
        var baseName = Path.GetFileNameWithoutExtension(fileName).Trim();
        var safeBaseName = SafeFileNameRegex().Replace(baseName, "-").Trim('-', '.');
        return $"{(string.IsNullOrWhiteSpace(safeBaseName) ? "image" : safeBaseName)}-{Guid.NewGuid():N}{extension}";
    }

    private static async Task<bool> HasValidSignatureAsync(Stream stream, string extension, CancellationToken cancellationToken)
    {
        var buffer = new byte[12];
        var read = await ReadHeaderAsync(stream, buffer, cancellationToken);

        return extension switch
        {
            ".jpg" or ".jpeg" => read >= 3 && buffer[0] == 0xff && buffer[1] == 0xd8 && buffer[2] == 0xff,
            ".png" => read >= 8
                && buffer[0] == 0x89
                && buffer[1] == 0x50
                && buffer[2] == 0x4e
                && buffer[3] == 0x47
                && buffer[4] == 0x0d
                && buffer[5] == 0x0a
                && buffer[6] == 0x1a
                && buffer[7] == 0x0a,
            ".gif" => read >= 6
                && buffer[0] == 0x47
                && buffer[1] == 0x49
                && buffer[2] == 0x46
                && buffer[3] == 0x38
                && (buffer[4] == 0x37 || buffer[4] == 0x39)
                && buffer[5] == 0x61,
            ".webp" => read >= 12
                && buffer[0] == 0x52
                && buffer[1] == 0x49
                && buffer[2] == 0x46
                && buffer[3] == 0x46
                && buffer[8] == 0x57
                && buffer[9] == 0x45
                && buffer[10] == 0x42
                && buffer[11] == 0x50,
            _ => false
        };
    }

    private static async Task<int> ReadHeaderAsync(Stream stream, byte[] buffer, CancellationToken cancellationToken)
    {
        var totalRead = 0;
        while (totalRead < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(totalRead), cancellationToken);
            if (read == 0)
            {
                break;
            }

            totalRead += read;
        }

        return totalRead;
    }

    [GeneratedRegex("[^a-zA-Z0-9._-]+")]
    private static partial Regex SafeFileNameRegex();
}
