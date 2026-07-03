using PluginLib;
using System.Reflection;

namespace PluginLoader;

public class PluginManager
{
    private readonly List<IPlugin> _loadedPlugins = new();

    public IReadOnlyList<IPlugin> LoadedPlugins => _loadedPlugins.AsReadOnly();

    public void LoadPluginsFromDirectory(string directoryPath)
    {
        _loadedPlugins.Clear();
        if (!Directory.Exists(directoryPath)) return;

        var pluginTypes = new List<Type>();
        var dllFiles = Directory.GetFiles(directoryPath, "*.dll");

        foreach (var dll in dllFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(dll);
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IPlugin).IsAssignableFrom(type) &&
                        !type.IsAbstract &&
                        !type.IsInterface &&
                        type.GetCustomAttribute<PluginLoadAttribute>() != null)
                    {
                        pluginTypes.Add(type);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки {dll}: {ex.Message}");
            }
        }

        var graph = BuildDependencyGraph(pluginTypes);

        var sortedTypes = TopologicalSort(graph);

        foreach (var type in sortedTypes)
        {
            var instance = Activator.CreateInstance(type) as IPlugin;
            if (instance != null)
            {
                _loadedPlugins.Add(instance);
            }
        }
    }

    private Dictionary<Type, List<string>> BuildDependencyGraph(List<Type> pluginTypes)
    {
        var graph = new Dictionary<Type, List<string>>();
        var nameToType = pluginTypes.ToDictionary(
            t => ((IPlugin)Activator.CreateInstance(t)!).Name,
            t => t
        );

        foreach (var type in pluginTypes)
        {
            var dependencies = type.GetCustomAttributes<PluginDependencyAttribute>()
                .Select(attr => attr.RequiredPluginName)
                .ToList();
            graph[type] = dependencies;
        }

        return graph;
    }

    private List<Type> TopologicalSort(Dictionary<Type, List<string>> graph)
    {
        var result = new List<Type>();
        var visited = new HashSet<Type>();
        var visiting = new HashSet<Type>();

        var nameToType = graph.Keys.ToDictionary(
            t => ((IPlugin)Activator.CreateInstance(t)!).Name,
            t => t
        );

        foreach (var type in graph.Keys)
        {
            if (!visited.Contains(type))
            {
                Visit(type, graph, visited, visiting, result, nameToType);
            }
        }

        return result;
    }

    private void Visit(
        Type type,
        Dictionary<Type, List<string>> graph,
        HashSet<Type> visited,
        HashSet<Type> visiting,
        List<Type> result,
        Dictionary<string, Type> nameToType)
    {
        if (visiting.Contains(type))
        {
            throw new InvalidOperationException($"Циклическая зависимость обнаружена для {type.Name}");
        }

        if (visited.Contains(type))
        {
            return;
        }

        visiting.Add(type);

        if (graph.TryGetValue(type, out var dependencies))
        {
            foreach (var depName in dependencies)
            {
                if (nameToType.TryGetValue(depName, out var depType))
                {
                    Visit(depType, graph, visited, visiting, result, nameToType);
                }
            }
        }

        visiting.Remove(type);
        visited.Add(type);
        result.Add(type);
    }

    public void ExecuteAll()
    {
        foreach (var plugin in _loadedPlugins)
        {
            plugin.Execute();
        }
    }
}
