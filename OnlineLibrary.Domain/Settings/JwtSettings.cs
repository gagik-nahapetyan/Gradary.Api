namespace OnlineLibrary.Domain.Settings;

/// <summary>
/// Represents the JWT configuration settings.
/// </summary>
public class JwtSettings
{
    /// <summary>The token issuer.</summary>
    public required string Issuer { get; set; }

    /// <summary>The token audience.</summary>
    public required string Audience { get; set; }

    /// <summary>The signing secret key (min 32 characters).</summary>
    public required string SecretKey { get; set; }

    /// <summary>Access token lifetime in minutes.</summary>
    public int AccessTokenExpiryMinutes { get; set; }
}
