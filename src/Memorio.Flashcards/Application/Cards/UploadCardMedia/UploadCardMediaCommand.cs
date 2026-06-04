using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Microsoft.AspNetCore.Http;

namespace Memorio.Flashcards.Application.Cards.UploadCardMedia;

public sealed record UploadCardMediaCommand(
    Guid UserId,
    Guid CardId,
    IFormFile File) : IRequest<ErrorOr<CardMediaDto>>;
