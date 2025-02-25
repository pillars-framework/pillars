namespace Pillars.Chat.Controllers;

[RegisterSingleton]
public sealed class ChatController
{
	private readonly ILogger _logger;
	private readonly PiChatActor _piChatActor;
	private readonly Dictionary<string, MethodInfo> _registeredCommands = [];
	private readonly IServiceProvider _serviceProvider;

	public ChatController(ILogger l, ChatEvents ce, PiChatActor pica, IServiceProvider sp)
	{
		_logger = l.ForThisContext();
		_piChatActor = pica;
		_serviceProvider = sp;
		ce.OnChatMessage += OnChatMessage;
		RegisterCommands();
	}

	/// <summary>
	/// Scans the current assembly for types decorated with the <see cref="SlashCommandAttribute"/>
	/// and registers them as slash commands. These commands are stored in a private collection
	/// for later retrieval and use.
	/// </summary>
	/// <remarks>
	/// This method is typically called during application initialization to discover and register
	/// all available slash commands defined in the application. Only types marked with the
	/// <see cref="SlashCommandAttribute"/> are considered valid slash commands.
	/// </remarks>
	private void RegisterCommands()
	{
		foreach (var method in Assembly.GetExecutingAssembly().GetTypes()
			         .SelectMany(t =>
				         t.GetMethods(BindingFlags.Public | BindingFlags.Instance |
				                      BindingFlags.Static))
			         .Where(m => m.GetCustomAttribute<SlashCommandAttribute>() is not null))
		{
			var attribute = method.GetCustomAttribute<SlashCommandAttribute>();
			_registeredCommands.Add(attribute!.Identifier, method);
		}

		_logger.Information("Registered {amount} slash commands", _registeredCommands.Count);
	}

	/// <summary>
	/// Handles incoming chat messages from players and processes them as commands if they start with the '/' prefix.
	/// </summary>
	/// <param name="player">The player who sent the chat message.</param>
	/// <param name="message">The chat message sent by the player.</param>
	/// <remarks>
	/// This method checks if the message starts with the '/' character, indicating it is a command.
	/// If it is a command, the method extracts the command identifier (the first word after the '/')
	/// and looks it up in the registered commands collection (<see cref="_registeredCommands"/>).
	///
	/// If the command is found, it resolves the associated command handler from the dependency injection
	/// container (<see cref="_serviceProvider"/>) and invokes the command method, passing the player as an argument.
	///
	/// If the command is not found, an error message is sent back to the player indicating that the command does not exist.
	/// </remarks>
	private async Task OnChatMessage(PiPlayer player, string message)
	{
		if (!message.StartsWith('/')) return;

		var identifier = message.Split(' ')[0][1..];
		if (!_registeredCommands.TryGetValue(identifier, out var command))
			_piChatActor.SendMessageToPlayer(player.Native, $"The command {identifier} does not exist.");
		else
		{
			var instance = _serviceProvider.GetRequiredService(command.DeclaringType!);
			if (instance is null) return;
			command.Invoke(instance, [player]);
		}
	}
}
