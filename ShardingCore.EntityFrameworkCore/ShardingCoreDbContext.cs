using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.Domain;
using ShardingCore.EntityFrameworkCore.EntityTypeConfigurations;
using ShardingCore.Sharding;
using ShardingCore.Sharding.Abstractions;

namespace ShardingCore.EntityFrameworkCore;

public class ShardingCoreDbContext : AbstractShardingDbContext, IShardingTableDbContext
{
    public ShardingCoreDbContext(DbContextOptions options) : base(options)
    {
    }

    public IRouteTail RouteTail { get; set; } = default!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<string>().HaveMaxLength(50);
        configurationBuilder.Properties<decimal>().HaveColumnType("decimal(18,4)");

        configurationBuilder.AddStronglyTypedId(typeof(OrderId).Assembly);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrderMap());
    }
}