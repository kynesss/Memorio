using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;

namespace Memorio.Users.Application.Abstractions;

public interface IAuthTokenIssuer
{
    Task<AuthResponse> IssueAsync(ApplicationUser user, CancellationToken cancellationToken);
}
