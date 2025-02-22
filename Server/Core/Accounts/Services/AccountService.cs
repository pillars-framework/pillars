namespace Pillars.Core.Accounts.Services;

/// <summary>
/// The account service handles the database CRUD operations.
/// </summary>
[RegisterSingleton]
public sealed class AccountService(ILogger l)
{
	private readonly ILogger _logger = l.ForThisContext();

	#region CREATE

	/// <summary>
	/// Creates an empty account using the discordId.
	/// Does NOT check for existance of an already created account.
	/// </summary>
	public async Task<Account?> CreateAccount(ulong discordId)
	{
		try
		{
			Account acc = new() { DiscordId = discordId };
			await DB.SaveAsync(acc);
			return acc;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return null;
		}
	}

	#endregion

	#region READ

	/// <summary>
	/// Returns the a possible account for a given discordId.
	/// Returns null on error if no account is present.
	/// </summary>
	public async Task<Account?> GetAccountAsync(ulong discordId)
	{
		try
		{
			return await DB.Find<Account>().Match(a => a.DiscordId == discordId).ExecuteFirstAsync();
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return null;
		}
	}

	/// <summary>
	/// Returns the a possible account for a given accountId.
	/// Returns null on error if no account is present.
	/// </summary>
	public async Task<Account?> GetAccountAsync(string accountId)
	{
		try
		{
			return await DB.Find<Account>().OneAsync(accountId);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return null;
		}
	}

	#endregion
}
