using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Memorio.Shared.Security;

public sealed class CurrentUserContext : IUserContext
{
    private const string SubjectClaimType = "sub";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public Guid? UserId
    {
        get
        {
            var principal = _httpContextAccessor.HttpContext?.User;

            var subject = principal?.FindFirstValue(SubjectClaimType)
                ?? principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(subject, out var userId) ? userId : null;
        }
    }
}
