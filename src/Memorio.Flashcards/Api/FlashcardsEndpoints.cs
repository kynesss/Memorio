using Microsoft.AspNetCore.Routing;

namespace Memorio.Flashcards.Api;

public static class FlashcardsEndpoints
{
    public static IEndpointRouteBuilder MapFlashcardsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDeckEndpoints();
        endpoints.MapCardEndpoints();

        return endpoints;
    }
}
