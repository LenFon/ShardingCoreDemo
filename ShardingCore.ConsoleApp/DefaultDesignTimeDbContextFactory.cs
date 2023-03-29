using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore.EntityFrameworkCore;

namespace ShardingCore.ConsoleApp
{
    internal class DefaultDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ShardingCoreDbContext>
    {
        private static readonly IServiceProvider _serviceProvider;

        static DefaultDesignTimeDbContextFactory()
        {
            var services = new ServiceCollection();

            services.AddShardingCoreDbContext(o =>
            {
                //use your data base connection string
                o.AddDefaultDataSource(Guid.NewGuid().ToString("n"),
                    "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreShardingTableDB;User ID=sa;Password=1;");
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        public ShardingCoreDbContext CreateDbContext(string[] args)
        {
            return _serviceProvider.GetRequiredService<ShardingCoreDbContext>();
        }
    }
}
