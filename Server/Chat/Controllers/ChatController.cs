namespace Pillars.Chat.Controllers;

[RegisterSingleton]
public sealed class ChatController
{
	private readonly ILogger _logger;
	private readonly ChatActor _chatActor;
	private readonly Dictionary<string, MethodInfo> _registeredCommands = [];
	private readonly IServiceProvider _serviceProvider;

	public ChatController(ILogger l, ChatEvents ce, ChatActor ca, IServiceProvider sp)
	{
		_logger = l.ForThisContext();
		_chatActor = ca;
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
				                      BindingFlags.Static | BindingFlags.NonPublic))
			         .Where(m => m.GetCustomAttribute<SlashCommandAttribute>() is not null))
		{
			var attr = method.GetCustomAttribute<SlashCommandAttribute>();
			if (attr is null) continue;
			string command = attr.Identifier.ToLower(CultureInfo.CurrentCulture);
			if (command.StartsWith('/'))
				throw new InitializationException(typeof(ChatController),
					$"\"{command} should not start with \"/\"'");
			if (!HasValidParameters(attr, method))
				throw new InitializationException(typeof(ChatController),
					$"\"{command} has invalid parameter declaration'");
			if (!_registeredCommands.TryAdd(command, method))
				throw new InitializationException(typeof(ChatController),
					$"\"{command}\" is already a registered command");
		}

		_logger.Information("Registered {amount} slash commands", _registeredCommands.Count);
	}

	/// <summary>
	/// Checks if the given method info is a valid command registration by checking the attribute. <br/>
	/// If first argument is not a player, prints a warning.
	/// Returns true if valid
	/// </summary>
	/// <param name="attr"></param>
	/// <param name="mi"></param>
	/// <returns></returns>
	private bool HasValidParameters(SlashCommandAttribute attr, MethodInfo mi)
	{
		ParameterInfo[] @params = mi.GetParameters();
		if (@params.Length == 2 && @params[0].ParameterType == typeof(PiPlayer) &&
		    @params[1].ParameterType == typeof(string))
			return true;
		_logger.Warning(
			$"Command {attr.Identifier} - The method {mi.Name} of {mi.DeclaringType?.Name} must have PiPlayer as its first and \"string\" as a second parameter!");
		return false;
	}

	private readonly CompositeFormat _unknownCommandFormat = CompositeFormat.Parse(
		new ChatMessage.Builder()
			.AddSender("Error", CHATTEXTSTYLE.RED)
			.AddText("Unknown command: {0}")
			.Build().Message
	);

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
			_chatActor.SendMessageToPlayer(player,
				string.Format(CultureInfo.InvariantCulture, _unknownCommandFormat, $"/{identifier}"));
		else
		{
			var instance = _serviceProvider.GetRequiredService(command.DeclaringType!);
			if (instance is null) return;
			command.Invoke(instance, [player, message]);
		}
	}
}
