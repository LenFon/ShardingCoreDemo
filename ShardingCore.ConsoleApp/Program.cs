using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore;
using ShardingCore.Domain;
using ShardingCore.EntityFrameworkCore;

var services = new ServiceCollection();

services.AddShardingCoreDbContext(o =>
{
    //use your data base connection string
    o.AddDefaultDataSource(Guid.NewGuid().ToString("n"),
        "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EfCoreShardingTableDb;User ID=sa;Password=1;");
});

var app = services.BuildServiceProvider();

//not required, enable check table missing and auto create,非必须  启动检查缺少的表并且创建
app.UseAutoTryCompensateTable();

var db = app.GetRequiredService<ShardingCoreDbContext>();
var order = new Order
{
    Id = Guid.NewGuid(),
    Buyer = new Buyer { Id = Guid.NewGuid(), Name = "buyer" },
    Seller = new Seller { Id = Guid.NewGuid(), Name = "seller" },
    ReceiverAddress = new ReceiverAddress { Id = Guid.NewGuid(), Province = "r01", City = "r02", Area = "r03", Other = "xxx1" },
    DeliveryAddress = new DeliveryAddress { Id = Guid.NewGuid(), Province = "d01", City = "d02", Area = "d03", Other = "xxx2" },
    Products = new HashSet<Product>
    {
        new Product{ Key=Guid.NewGuid(),Name="product 1", Price=20,Unit="件",Quantity=10 },
        new Product{ Key=Guid.NewGuid(),Name="product 2", Price=10,Unit="件",Quantity=1 },
    },
    OrderStatus = OrderStatus.Created,
    PayStatus = PayStatus.NoPay,
    CreationTime = DateTime.Now,
};

await db.AddAsync(order);
await db.SaveChangesAsync();

Console.WriteLine("end");
Console.Read();