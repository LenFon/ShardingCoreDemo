using ShardingCore.Domain;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace ShardingCore.Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStronglyTypedIdTypeConverter(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            var infos = assemblies.SelectMany(s => s.GetTypes())
                .Where(w => !w.IsAbstract && !w.IsInterface && !w.IsGenericType)
                .Select(s => new
                {
                    StronglyTypedIdType = s,
                    StronglyTypedIdInterfaceType = s.GetInterfaces().FirstOrDefault(w => w.IsGenericType && w.GetGenericTypeDefinition() == typeof(IStronglyTypedId<,>))
                })
                .Where(w => w.StronglyTypedIdInterfaceType is not null)
                .ToList();

            foreach (var info in infos)
            {
                var converter = typeof(StronglyTypedIdTypeConverter<,>).MakeGenericType(info.StronglyTypedIdInterfaceType.GenericTypeArguments);

                TypeDescriptor.AddAttributes(info.StronglyTypedIdType, new TypeConverterAttribute(converter));
            }

            return services;
        }

        class StronglyTypedIdTypeConverter<TStronglyTypedId, TPrimitiveId> : TypeConverter
            where TStronglyTypedId : IStronglyTypedId<TStronglyTypedId, TPrimitiveId>
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
                if (TPrimitiveId.TryParse(value.ToString(), null, out var val))
                {
                    return TStronglyTypedId.Create(val);
                }

                return base.ConvertFrom(context, culture, value);
            }

            public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));

                if (value is IStronglyTypedId<TStronglyTypedId, TPrimitiveId> id)
                {
                    if (destinationType == typeof(string))
                    {
                        return id.Value.ToString();
                    }
                    if (destinationType == typeof(TPrimitiveId))
                    {
                        return id.Value;
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
