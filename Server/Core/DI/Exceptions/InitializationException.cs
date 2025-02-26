namespace Pillars.Core.DI.Exceptions;

/// <summary>
/// Used in controllers to throw exception and abort the startup
/// </summary>
public sealed class InitializationException : Exception
{
	public InitializationException(Type type)
		: base($"An error occurred during initialization of {type.Name}. Aborting startup...")
	{
	}

	public InitializationException(Type type, string customMessage)
		: base($"An error occurred during initialization of {type.Name}. {customMessage}. Aborting startup...")
	{
	}

	public InitializationException(Type type, string customMessage, Exception innerException)
		: base($"An error occurred during initialization of {type.Name}. {customMessage}. Aborting startup...",
			innerException)
	{
	}
}
