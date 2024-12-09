namespace Application.Commands.Auth.Login;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResult>;

public sealed record LoginUserResult(string Token);