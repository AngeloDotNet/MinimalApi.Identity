using Microsoft.Extensions.Options;

namespace MinimalApi.Identity.API.Options;

public class AuthOptions(IOptions<JwtOptions> jOptions, IOptions<NetIdentityOptions> iOptions, IOptions<UsersOptions> uOptions)
{
    public JwtOptions Jwt { get; } = jOptions.Value;
    public NetIdentityOptions NetIdentity { get; } = iOptions.Value;
    public UsersOptions Users { get; } = uOptions.Value;
}
