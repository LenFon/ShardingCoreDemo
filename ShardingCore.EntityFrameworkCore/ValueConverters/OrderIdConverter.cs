using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShardingCore.Domain;

namespace ShardingCore.EntityFrameworkCore.ValueConverters;

internal class OrderIdConverter : ValueConverter<OrderId, Guid>
{
    public OrderIdConverter() : base(v => v.Value, v => new(v))
    {
    }
}