using Microsoft.OpenApi.Models;
using ShardingCore.Domain;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Concurrent;
using System.Reflection;

namespace ShardingCore.Web;

public static class SwaggerGenOptionsExtensions
{
    private static readonly ConcurrentDictionary<string, Func<OpenApiSchema>> OpenApiSchemaCache = new()
    {
        ["string"] = () => new OpenApiSchema { Type = "string" },
        ["int32"] = () => new OpenApiSchema { Type = "integer", Format = "int32" },
        ["int64"] = () => new OpenApiSchema { Type = "integer", Format = "int64" },
        ["uuid"] = () => new OpenApiSchema { Type = "string", Format = "uuid" },
        ["float"] = () => new OpenApiSchema { Type = "number", Format = "float" },
        ["double"] = () => new OpenApiSchema { Type = "number", Format = "double" },
    };

    private static readonly ConcurrentDictionary<Type, string> Cache = new()
    {
        [typeof(int)] = "int32",
        [typeof(uint)] = "int32",
        [typeof(byte)] = "int32",
        [typeof(sbyte)] = "int32",
        [typeof(bool)] = "int32",
        [typeof(short)] = "int32",
        [typeof(ushort)] = "int32",

        [typeof(long)] = "int64",
        [typeof(ulong)] = "int64",

        [typeof(Guid)] = "uuid",

        [typeof(string)] = "string",
        [typeof(char)] = "string",

        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "double",
    };

    public static void MapTypeOfStronglyTypedId(this SwaggerGenOptions c, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0) return;

        MapTypeOfStronglyTypedId(c, assemblies.SelectMany(s => s.GetTypes()));
    }

    public static void MapTypeOfStronglyTypedId(this SwaggerGenOptions c, IEnumerable<Assembly> assemblies)
    {
        if (assemblies == null || !assemblies.Any()) return;

        MapTypeOfStronglyTypedId(c, assemblies.SelectMany(s => s.GetTypes()));
    }

    public static void MapTypeOfStronglyTypedId(this SwaggerGenOptions c, params Type[] stronglyTypedIdTypes)
    {
        if (stronglyTypedIdTypes == null || stronglyTypedIdTypes.Length == 0) return;

        foreach (var stronglyTypedIdType in stronglyTypedIdTypes)
        {
            MapTypeOfStronglyTypedId(c, stronglyTypedIdType);
        }
    }

    public static void MapTypeOfStronglyTypedId(this SwaggerGenOptions c, IEnumerable<Type> stronglyTypedIdTypes)
    {
        if (stronglyTypedIdTypes == null || !stronglyTypedIdTypes.Any()) return;

        foreach (var stronglyTypedIdType in stronglyTypedIdTypes)
        {
            MapTypeOfStronglyTypedId(c, stronglyTypedIdType);
        }
    }

    public static void MapTypeOfStronglyTypedId(this SwaggerGenOptions options, Type stronglyTypedIdType)
    {
        if (IsStronglyTypedId(stronglyTypedIdType, out var primitiveIdType))
        {
            if (Cache.TryGetValue(primitiveIdType!, out var value))
            {
                options.MapType(stronglyTypedIdType, OpenApiSchemaCache[value]);
            }
            else
            {
                options.MapType(stronglyTypedIdType, () => new OpenApiSchema());
            }
        }
    }

    private static bool IsStronglyTypedId(Type type, out Type? primitiveIdType)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        if (type.GetInterfaces()
            .FirstOrDefault(w =>
                w.IsGenericType &&
                w.GetGenericTypeDefinition() == typeof(IStronglyTypedId<>)) is Type stronglyTypedIdInterfaceType)
        {
            var arguments = stronglyTypedIdInterfaceType.GetGenericArguments();
            primitiveIdType = arguments[0];

            return true;
        }

        primitiveIdType = null;

        return false;
    }
}
