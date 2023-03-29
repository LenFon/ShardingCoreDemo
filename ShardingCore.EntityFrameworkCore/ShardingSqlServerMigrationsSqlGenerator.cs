using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Update;
using ShardingCore.Core.RuntimeContexts;
using ShardingCore.Helpers;
using ShardingCore.Sharding.Abstractions;

namespace ShardingCore.EntityFrameworkCore
{
    internal class ShardingSqlServerMigrationsSqlGenerator<TShardingDbContext>
        : SqlServerMigrationsSqlGenerator where TShardingDbContext : DbContext, IShardingDbContext
    {
        private readonly IShardingRuntimeContext _shardingRuntimeContext;
        public ShardingSqlServerMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, ICommandBatchPreparer commandBatchPreparer, IShardingRuntimeContext shardingRuntimeContext)
            : base(dependencies, commandBatchPreparer)
        {
            _shardingRuntimeContext = shardingRuntimeContext;
        }

        protected override void Generate(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            var oldCmds = builder.GetCommandList().ToList();
            base.Generate(operation, model, builder);
            var newCmds = builder.GetCommandList().ToList();
            var addCmds = newCmds.Where(x => !oldCmds.Contains(x)).ToList();

            MigrationHelper.Generate(_shardingRuntimeContext, operation, builder, Dependencies.SqlGenerationHelper, addCmds);
        }
    }
}
