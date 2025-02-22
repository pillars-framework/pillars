namespace Pillars.Core.Accounts.Controllers;

[RegisterSingleton]
public sealed class AccountController(ILogger l, AccountService acs)
{
	private readonly ILogger _logger = l.ForThisContext();

	/// <summary>
	/// For a given discordId, either retrieves, or creates a new account.
	/// Returns null in case of an error.
	/// </summary>
	public async Task<Account?> GetOrCreateAccountAsync(ulong discordId)
	{
		try
		{
			return await acs.GetAccountAsync(discordId) ??
			       await acs.CreateAccount(discordId);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return null;
		}
	}
}
