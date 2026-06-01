using MediatR;
using Memorio.Shared.Exceptions;
using Memorio.Users.Api.Requests;
using Memorio.Users.Application.Abstractions;
using Memorio.Users.Application.Auth.GetCurrentUser;
using Memorio.Users.Application.Auth.Login;
using Memorio.Users.Application.Auth.Refresh;
using Memorio.Users.Application.Auth.Register;
using Memorio.Users.Application.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Users.Api;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/register", async (RegisterRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new RegisterUserCommand(request.Email, request.Password), cancellationToken);
            return Results.Ok(response);
        })
        .WithName("RegisterUser")
        .WithSummary("Rejestruje nowego użytkownika i zwraca parę tokenów.")
        .Produces<AuthResponse>()
        .ProducesValidationProblem();

        group.MapPost("/login", async (LoginRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new LoginUserCommand(request.Email, request.Password), cancellationToken);
            return Results.Ok(response);
        })
        .WithName("LoginUser")
        .WithSummary("Loguje użytkownika i zwraca access oraz refresh token.")
        .Produces<AuthResponse>()
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", async (RefreshRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new RefreshTokenCommand(request.RefreshToken), cancellationToken);
            return Results.Ok(response);
        })
        .WithName("RefreshToken")
        .WithSummary("Wymienia refresh token na nową parę tokenów.")
        .Produces<AuthResponse>()
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", async (IUserContext userContext, ISender sender, CancellationToken cancellationToken) =>
        {
            var userId = userContext.UserId ?? throw new UnauthorizedException();
            var response = await sender.Send(new GetCurrentUserQuery(userId), cancellationToken);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser")
        .WithSummary("Zwraca dane zalogowanego użytkownika.")
        .Produces<UserResponse>()
        .Produces(StatusCodes.Status401Unauthorized);

        return endpoints;
    }
}
