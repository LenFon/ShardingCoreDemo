using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore.Core.ShardingConfigurations;
using ShardingCore.EntityFrameworkCore.VirtualTableRoutes;

namespace ShardingCore.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShardingCoreDbContext(this IServiceCollection services, Action<ShardingConfigOptions>? shardingConfigure = null)
    {
        services.AddShardingDbContext<ShardingCoreDbContext>()
            .UseConfig(o =>
            {
                shardingConfigure?.Invoke(o);

                o.UseShardingQuery((connStr, builder) =>
                {
                    //connStr is delegate input param
                    builder.UseSqlServer(connStr, b => b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                    builder.EnableSensitiveDataLogging();

                });
                o.UseShardingTransaction((connection, builder) =>
                {
                    //connection is delegate input param
                    builder.UseSqlServer(connection, b => b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                });

                o.UseShardingMigrationConfigure(b =>
                {
                    b.ReplaceService<IMigrationsSqlGenerator, ShardingSqlServerMigrationsSqlGenerator<ShardingCoreDbContext>>();
                });
            })
            .UseRouteConfig(o =>
            {
                o.AddShardingTableRoute<OrderVirtualTableRoute>();
            })
            .AddShardingCore();
        return services;
    }
}
