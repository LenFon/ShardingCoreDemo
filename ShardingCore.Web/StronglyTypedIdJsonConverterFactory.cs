using ShardingCore.Domain;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShardingCore.Web;

public class StronglyTypedIdJsonConverterFactory : JsonConverterFactory
{
    private static readonly ConcurrentDictionary<Type, JsonConverter> Cache = new();

    public override bool CanConvert(Type typeToConvert) => IsStronglyTypedId(typeToConvert);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        Cache.GetOrAdd(typeToConvert, CreateConverter);

    private JsonConverter CreateConverter(Type typeToConvert)
    {
        if (!IsStronglyTypedId(typeToConvert, out var _, out var primitiveIdType))
            throw new InvalidOperationException($"Cannot create converter for '{typeToConvert}'");

        var type = typeof(StronglyTypedIdJsonConverter<,>).MakeGenericType(typeToConvert, primitiveIdType!);
        return (JsonConverter)Activator.CreateInstance(type)!;
    }

    private static bool IsStronglyTypedId(Type type) => IsStronglyTypedId(type, out var _, out var _);

    private static bool IsStronglyTypedId(Type type, out Type? stronglyTypedIdType, out Type? primitiveIdType)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        if (type.GetInterfaces()
            .FirstOrDefault(w =>
                w.IsGenericType &&
                w.GetGenericTypeDefinition() == typeof(IStronglyTypedId<,>)) is Type stronglyTypedIdInterfaceType)
        {
            var arguments = stronglyTypedIdInterfaceType.GetGenericArguments();
            stronglyTypedIdType = arguments[0];
            primitiveIdType = arguments[1];

            return true;
        }

        stronglyTypedIdType = null;
        primitiveIdType = null;

        return false;
    }

    private class StronglyTypedIdJsonConverter<TStrongTypedId, TPrimitiveId> : JsonConverter<TStrongTypedId>
        where TStrongTypedId : IStronglyTypedId<TStrongTypedId, TPrimitiveId>
        where TPrimitiveId : struct, IComparable, IComparable<TPrimitiveId>, IEquatable<TPrimitiveId>, ISpanParsable<TPrimitiveId>
    {
        public override TStrongTypedId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = (TPrimitiveId)GetValue(reader);
            return TStrongTypedId.Create(value);
        }

        private static object GetValue(Utf8JsonReader reader)
        {
            return typeof(TPrimitiveId) switch
            {
                { } t when t == typeof(bool) => reader.GetBoolean(),
                { } t when t == typeof(Guid) => reader.GetGuid(),
                { } t when t == typeof(short) => reader.GetInt16(),
                { } t when t == typeof(int) => reader.GetInt32(),
                { } t when t == typeof(long) => reader.GetInt64(),
                { } t when t == typeof(ushort) => reader.GetUInt16(),
                { } t when t == typeof(uint) => reader.GetUInt32(),
                { } t when t == typeof(ulong) => reader.GetUInt64(),
                { } t when t == typeof(float) => reader.GetSingle(),
                { } t when t == typeof(double) => reader.GetDouble(),
                { } t when t == typeof(decimal) => reader.GetDecimal(),
                { } t when t == typeof(byte) => reader.GetByte(),
                { } t when t == typeof(sbyte) => reader.GetSByte(),
                _ => throw new NotSupportedException()
            };
        }

        public override void Write(Utf8JsonWriter writer, TStrongTypedId value, JsonSerializerOptions options)
        {
            var writeAction = GetWriteAction(writer, value);
            writeAction();
        }

        private static Action GetWriteAction(Utf8JsonWriter writer, TStrongTypedId value)
        {
            return value.Value switch
            {
                bool val => () => writer.WriteBooleanValue(val),
                Guid val => () => writer.WriteStringValue(val.ToString()),
                short val => () => writer.WriteNumberValue(val),
                int val => () => writer.WriteNumberValue(val),
                long val => () => writer.WriteNumberValue(val),
                ushort val => () => writer.WriteNumberValue(val),
                uint val => () => writer.WriteNumberValue(val),
                ulong val => () => writer.WriteNumberValue(val),
                float val => () => writer.WriteNumberValue(val),
                double val => () => writer.WriteNumberValue(val),
                decimal val => () => writer.WriteNumberValue(val),
                byte val => () => writer.WriteNumberValue(val),
                sbyte val => () => writer.WriteNumberValue(val),
                _ => throw new NotSupportedException()
            };
        }
    }
}
