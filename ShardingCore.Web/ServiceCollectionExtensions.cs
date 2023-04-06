using ShardingCore.Domain;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace ShardingCore.Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStronglyTypedIdTypeConverter(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies == null || !assemblies.Any())
                return services;

            return AddStronglyTypedIdTypeConverter(services, assemblies.SelectMany(s => s.GetTypes()));
        }

        public static IServiceCollection AddStronglyTypedIdTypeConverter(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null || !assemblies.Any())
                return services;

            return AddStronglyTypedIdTypeConverter(services, assemblies.SelectMany(s => s.GetTypes()));
        }

        public static IServiceCollection AddStronglyTypedIdTypeConverter(this IServiceCollection services, params Type[] stronglyTypedIdTypes)
        {
            if (stronglyTypedIdTypes == null || !stronglyTypedIdTypes.Any())
                return services;

            foreach (var stronglyTypedIdType in stronglyTypedIdTypes)
            {
                AddStronglyTypedIdTypeConverter(services, stronglyTypedIdType);
            }

            return services;
        }


        public static IServiceCollection AddStronglyTypedIdTypeConverter(this IServiceCollection services, IEnumerable<Type> stronglyTypedIdTypes)
        {
            if (stronglyTypedIdTypes == null || !stronglyTypedIdTypes.Any())
                return services;

            foreach (var stronglyTypedIdType in stronglyTypedIdTypes)
            {
                AddStronglyTypedIdTypeConverter(services, stronglyTypedIdType);
            }

            return services;
        }

        public static IServiceCollection AddStronglyTypedIdTypeConverter(this IServiceCollection services, Type stronglyTypedIdType)
        {
            if (IsStronglyTypedId(stronglyTypedIdType, out var primitiveIdType))
            {
                var converter = typeof(StronglyTypedIdTypeConverter<,>).MakeGenericType(stronglyTypedIdType, primitiveIdType!);

                TypeDescriptor.AddAttributes(stronglyTypedIdType, new TypeConverterAttribute(converter));
            }

            return services;
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

        class StronglyTypedIdTypeConverter<TStronglyTypedId, TPrimitiveId> : TypeConverter
            where TStronglyTypedId : IStronglyTypedId<TPrimitiveId>
            where TPrimitiveId : struct, IComparable, IComparable<TPrimitiveId>, IEquatable<TPrimitiveId>, ISpanParsable<TPrimitiveId>
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
                sourceType == typeof(string) ||
                sourceType == typeof(TPrimitiveId) ||
                base.CanConvertFrom(context, sourceType);

            public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType) =>
                destinationType == typeof(string) ||
                destinationType == typeof(TPrimitiveId) ||
                base.CanConvertTo(context, destinationType);

            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            {
                return value switch
                {
                    TPrimitiveId val => TStronglyTypedId.Create(val),
                    string val when string.IsNullOrEmpty(val) && TPrimitiveId.TryParse(val, null, out var result) =>
                        TStronglyTypedId.Create(result),
                    _ => base.ConvertFrom(context, culture, value),
                };
            }

            public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            {
                if (value is IStronglyTypedId<TPrimitiveId> id)
                {
                    if (destinationType == typeof(TPrimitiveId))
                    {
                        return id.Value;
                    }

                    if (destinationType == typeof(string))
                    {
                        return id.Value.ToString();
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
