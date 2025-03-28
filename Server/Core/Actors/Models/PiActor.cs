﻿namespace Pillars.Core.Actors.Models;

/// <summary>
/// Represents an abstract base class for actors in the game world.
/// </summary>
/// <typeparam name="T">The type of actor to spawn, which must derive from <see cref="Actor"/>.</typeparam>
/// <remarks>
/// This class is responsible for spawning an instance of the specified actor type (<typeparamref name="T"/>) in the game world.
/// If the actor cannot be spawned, an exception is thrown.
/// </remarks>
/// <exception cref="InitializationException">Thrown when the actor fails to spawn in the game world.</exception>
public abstract class PiActor<T> where T : Actor
{
	/// <summary>
	/// The spawned world actor instance.
	/// </summary>
	/// <value>
	/// An instance of type <typeparamref name="T"/> representing the spawned actor in the game world.
	/// </value>
	protected readonly T _worldActor = HogWarpSdk.Server.World.Spawn<T>() ??
	                                   throw new InitializationException(typeof(T), "Failed to spawn world actor");
}
