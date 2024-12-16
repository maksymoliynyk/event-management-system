namespace Infrastructure.Options;

public class ConnectionStringsOption
{
    public string DefaultConnection { get; init; }
    public string ReadOnlyConnection { get; init; }
}