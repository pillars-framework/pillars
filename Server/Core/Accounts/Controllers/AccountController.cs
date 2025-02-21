namespace Pillars.Core.Accounts.Controllers;

[RegisterSingleton]
public sealed class AccountController(ILogger l)
{
	private readonly ILogger _logger = l.ForThisContext();
}
