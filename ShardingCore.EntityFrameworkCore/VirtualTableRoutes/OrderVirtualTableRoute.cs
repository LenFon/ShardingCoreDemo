using ShardingCore.Core.EntityMetadatas;
using ShardingCore.Domain;
using ShardingCore.VirtualRoutes.Months;

namespace ShardingCore.EntityFrameworkCore.VirtualTableRoutes;

internal class OrderVirtualTableRoute : AbstractSimpleShardingMonthKeyDateTimeVirtualTableRoute<Order>
{
    public override bool AutoCreateTableByTime() => true;

    public override void Configure(EntityMetadataTableBuilder<Order> builder)
    {
        builder.ShardingProperty(o => o.CreationTime);
    }

    public override DateTime GetBeginTime() => new(2022, 1, 1);
}