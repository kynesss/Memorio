using System.Text;
using FluentValidation;
using LinqKit;
using Memorio.Users.Application.Abstractions;
using Memorio.Users.Domain;
using Memorio.Users.Infrastructure.Authentication;
using Memorio.Users.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Memorio.Users;

public static class UsersModule
{
    private const int MinimumSecretLengthInBytes = 32;

    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddIdentity(services);
        AddAuthentication(services, configuration);

        services.AddValidatorsFromAssembly(typeof(UsersModule).Assembly);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Database"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", UsersDbContext.Schema))
            .WithExpressionExpanding());

        services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();
    }

    private static void AddIdentity(IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<UsersDbContext>();
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = BuildJwtOptions(configuration);
        services.AddSingleton(jwtOptions);

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                };
            });

        services.AddAuthorization();
        services.AddScoped<IAccessTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthTokenIssuer, AuthTokenIssuer>();
    }

    private static JwtOptions BuildJwtOptions(IConfiguration configuration)
    {
        var options = new JwtOptions();
        configuration.GetSection(JwtOptions.SectionName).Bind(options);

        options.Secret = configuration["JWT_SECRET"] ?? options.Secret;
        options.Issuer = configuration["JWT_ISSUER"] ?? options.Issuer;
        options.Audience = configuration["JWT_AUDIENCE"] ?? options.Audience;

        if (Encoding.UTF8.GetByteCount(options.Secret) < MinimumSecretLengthInBytes)
        {
            throw new InvalidOperationException(
                $"JWT secret must be configured with at least {MinimumSecretLengthInBytes} bytes (set JWT_SECRET).");
        }

        return options;
    }
}
