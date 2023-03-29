using ShardingCore.Sharding.Abstractions;
using ShardingCore.Sharding;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.Domain;
using ShardingCore.EntityFrameworkCore.EntityTypeConfigurations;

namespace ShardingCore.EntityFrameworkCore;

public class ShardingCoreDbContext : AbstractShardingDbContext, IShardingTableDbContext
{
    public ShardingCoreDbContext(DbContextOptions options) : base(options)
    {
    }

    public IRouteTail RouteTail { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrderMap());
    }
}