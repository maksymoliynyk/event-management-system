namespace Infrastructure.Options;

public class JwtOption
{
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public string SecretKey { get; init; }
}