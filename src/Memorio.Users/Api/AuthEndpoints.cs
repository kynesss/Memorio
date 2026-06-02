using MediatR;
using Memorio.Shared.Exceptions;
using Memorio.Shared.Results;
using Memorio.Shared.Security;
using Memorio.Users.Api.Requests;
using Memorio.Users.Application.Auth.GetCurrentUser;
using Memorio.Users.Application.Auth.Login;
using Memorio.Users.Application.Auth.Refresh;
using Memorio.Users.Application.Auth.Register;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Infrastructure.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Users.Api;

public static class AuthEndpoints
{
    private const string RefreshTokenCookieName = "refresh_token";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/register", async (
            RegisterRequest request,
            ISender sender,
            HttpContext context,
            JwtOptions options,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new RegisterUserCommand(request.Email, request.Password), cancellationToken);
            return result.ToResponse(tokens => AccessTokenWithCookie(context, tokens, options));
        })
        .WithName("RegisterUser")
        .WithSummary("Rejestruje nowego użytkownika, ustawia refresh token w cookie i zwraca access token.")
        .Produces<AccessTokenResponse>()
        .ProducesValidationProblem();

        group.MapPost("/login", async (
            LoginRequest request,
            ISender sender,
            HttpContext context,
            JwtOptions options,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new LoginUserCommand(request.Email, request.Password), cancellationToken);
            return result.ToResponse(tokens => AccessTokenWithCookie(context, tokens, options));
        })
        .WithName("LoginUser")
        .WithSummary("Loguje użytkownika, ustawia refresh token w cookie i zwraca access token.")
        .Produces<AccessTokenResponse>()
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", async (
            ISender sender,
            HttpContext context,
            JwtOptions options,
            CancellationToken cancellationToken) =>
        {
            var refreshToken = context.Request.Cookies[RefreshTokenCookieName]
                ?? throw new UnauthorizedException("Refresh token is missing.");
            var result = await sender.Send(new RefreshTokenCommand(refreshToken), cancellationToken);
            return result.ToResponse(tokens => AccessTokenWithCookie(context, tokens, options));
        })
        .WithName("RefreshToken")
        .WithSummary("Wymienia refresh token z cookie na nowy access token i obraca cookie.")
        .Produces<AccessTokenResponse>()
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", async (IUserContext userContext, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCurrentUserQuery(userContext.RequireUserId()), cancellationToken);
            return result.ToResponse();
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser")
        .WithSummary("Zwraca dane zalogowanego użytkownika.")
        .Produces<UserResponse>()
        .Produces(StatusCodes.Status401Unauthorized);

        return endpoints;
    }

    private static IResult AccessTokenWithCookie(HttpContext context, AuthResponse tokens, JwtOptions options)
    {
        WriteRefreshTokenCookie(context.Response, tokens, options);
        return Results.Ok(new AccessTokenResponse(tokens.AccessToken, tokens.TokenType, tokens.ExpiresInSeconds));
    }

    private static void WriteRefreshTokenCookie(HttpResponse response, AuthResponse tokens, JwtOptions options)
    {
        response.Cookies.Append(RefreshTokenCookieName, tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = response.HttpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            MaxAge = options.RefreshTokenLifetime,
            Path = "/api/v1/auth"
        });
    }
}
