using System.Collections.Concurrent;
using System.Reflection;
using Airbnb.Domain;

namespace Airbnb.Infrastructure.Database.Dapper;

public static class QueryReader
{
    private static readonly Assembly Assembly = typeof(QueryReader).Assembly;
    private static readonly ConcurrentDictionary<string, string> Cache = new();

    public static string? GetQuery(string queryName)
    {
        Cache.TryGetValue(queryName, out var query);
        if (!string.IsNullOrEmpty(query)) return query;
        
        var loaded = LoadQuery(queryName);
        if (!string.IsNullOrEmpty(loaded.Value))
        {
            Cache.TryAdd(queryName, loaded.Value);
            return loaded.Value;
        }

        return null;
    }

    private static Result<string> LoadQuery(string queryName)
    {
        var suffix = $".{queryName}.sql";
        var resourceName = Assembly.GetManifestResourceNames()
            .SingleOrDefault(name => name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
        {
            return Result<string>.Fail($"SQL query '{queryName}' was not found as an embedded resource.");
        }

        using var stream = Assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return Result<string>.Success(reader.ReadToEnd());
    }
}
