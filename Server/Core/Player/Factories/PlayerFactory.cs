namespace Pillars.Core.Player.Factories;

/// <summary>
/// Factory for creating a PiPlayer, also injects the IServiceProvider
/// </summary>
[RegisterSingleton]
public sealed class PlayerFactory(IServiceProvider sp)
{
	public PiPlayer Create(NativePlayer native, Account acc) =>
		ActivatorUtilities.CreateInstance<PiPlayer>(sp, native, acc);
}
