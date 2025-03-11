namespace Pillars.Chat.Services;

[RegisterSingleton]
public sealed class ChatService(ILogger l)
{
	private readonly ILogger _logger = l.ForThisContext();

	/// <summary>
	/// For a given account id, returns the first found chat settings
	/// </summary>
	/// <param name="accountId">The account id to search for</param>
	/// <returns>The chat settings for the account or null on error or non-existing</returns>
	public async Task<ChatSettings?> GetSettingsForAccountIdAsync(string accountId)
	{
		try
		{
			return await DB.Find<ChatSettings>()
				.Match(cs => cs.Account.ID == accountId)
				.ExecuteFirstAsync();
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return null;
		}
	}
}
