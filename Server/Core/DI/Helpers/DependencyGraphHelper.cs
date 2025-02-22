namespace Pillars.Core.DI.Helpers;

public class DependencyGraphHelper
{
	private static readonly Dictionary<Type, HashSet<Type>> _dependencies = [];
	private static readonly ILogger _logger = Log.ForContext<DependencyGraphHelper>();

	/// <summary>
	/// Resolves all controller dependencies and initializes them.
	/// </summary>
	/// <param name="serviceProvider"></param>
	public static async Task ResolveControllerInitialization(IServiceProvider serviceProvider)
	{
		RegisterControllersFromAssembly();
		var orderedControllers = OrderControllersByDependencies()
			.Select(controllerType => serviceProvider.GetRequiredService(controllerType) as IController)
			.OfType<IController>();
		foreach (var controller in orderedControllers)
		{
			await controller.InitializeAsync();
			_logger.Information("{c} successfully initialized", controller.GetType().Name);
		}
	}

	/// <summary>
	/// Gets all controllers from assembly
	/// </summary>
	private static void RegisterControllersFromAssembly()
	{
		var controllerTypes = Assembly.GetExecutingAssembly().GetTypes()
			.Where(t => t.GetInterfaces().Contains(typeof(IController))).ToList();

		// Populate the dependency graph based on controller types
		foreach (var controllerType in controllerTypes)
		{
			var dependencies = GetControllerDependencies(controllerType);
			_dependencies[controllerType] = dependencies;
		}
	}

	/// <summary>
	/// Gets the dependencies of a controller by inspecting its constructor parameters.
	/// </summary>
	/// <param name="controllerType">The type of controller</param>
	/// <returns>A hashSet of dependencies</returns>
	private static HashSet<Type> GetControllerDependencies(Type controllerType)
	{
		var dependencies = new HashSet<Type>();

		// Inspect constructor parameters to determine dependencies
		var constructor = controllerType.GetConstructors().FirstOrDefault();
		if (constructor != null)
		{
			foreach (var parameter in constructor.GetParameters())
			{
				if (typeof(IController).IsAssignableFrom(parameter.ParameterType))
				{
					dependencies.Add(parameter.ParameterType);
				}
			}
		}

		return dependencies;
	}

	/// <summary>
	/// Orders controllers by dependencies.
	/// </summary>
	private static List<Type> OrderControllersByDependencies()
	{
		var sortedControllers = new List<Type>();
		var visited = new HashSet<Type>();

		foreach (var controllerType in _dependencies.Keys)
		{
			Visit(controllerType, _dependencies, visited, sortedControllers);
		}

		return sortedControllers;
	}

	/// <summary>
	/// Visits a controller and its dependencies.
	/// </summary>
	/// <param name="controllerType">Type of controller</param>
	/// <param name="dependencies">The dependencies of the controller</param>
	/// <param name="visited">If the controller was visited</param>
	/// <param name="sortedControllers">The sorted controllers</param>
	private static void Visit(Type controllerType, IReadOnlyDictionary<Type, HashSet<Type>> dependencies,
		ISet<Type> visited, ICollection<Type> sortedControllers)
	{
		if (!visited.Add(controllerType)) return;

		foreach (var dependency in dependencies[controllerType])
		{
			Visit(dependency, dependencies, visited, sortedControllers);
		}

		sortedControllers.Add(controllerType);
	}
}
