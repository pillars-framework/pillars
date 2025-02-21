namespace Pillars.Core.Accounts;

[RegisterSingleton]
public sealed class AccountService(ILogger l)
{
	private readonly ILogger _logger = l.ForThisContext();

	public async Task<Account?> GetAccountByDiscordId(ulong discordId)
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

	public async Task<Account?> GetAccountById(string id)
	{
		try
		{
			return await DB.Find<Account>().OneAsync(id);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return null;
		}
	}

	public async Task<Account?> CreateAccount(ulong discordId)
	{
		try
		{
			Account acc = new()
			{
				DiscordId = discordId,
				LastLogin = DateTime.Now
			};
			DB.SaveAsync(acc);
			return acc;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return null;
		}
	}


}
