namespace Pillars.Weather.Data;

/// <summary>
/// Represents the different seasons that can be applied in the game.
/// </summary>
/// <remarks>
/// This enum defines the possible seasons, including fall, winter, spring, and summer.
/// Each value corresponds to a specific season that can be set for the game world.
/// The <see cref="Invalid"/> value is used as a default or error state.
/// </remarks>
public enum SEASON
{
	Invalid = 0,
	Fall = 1,
	Winter = 2,
	Spring = 3,
	Summer = 4,
}
