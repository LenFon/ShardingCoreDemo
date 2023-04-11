using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Domain;
using ShardingCore.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShardingCore.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ShardingCoreDbContext _db;

        public OrdersController(ShardingCoreDbContext db)
        {
            _db = db;
        }

        // GET: api/<OrdersController>
        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            return await _db.Set<Order>().ToListAsync();
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public async Task<Order?> Get(OrderId id)
        {
            return await _db.Set<Order>().FirstOrDefaultAsync(w => w.Id == id);
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task Post([FromBody] string value)
        {
            var order = new Order
            {
                Id = new OrderId(Guid.NewGuid()),
                Buyer = new Account { Id = new AccountId(Guid.NewGuid()), Name = "buyer" },
                Seller = new Account { Id = new AccountId(Guid.NewGuid()), Name = "seller" },
                ReceiverAddress = new Address { Id = Guid.NewGuid(), Province = "r01", City = "r02", Area = "r03", Street = "r004", Other = "xxx1" },
                DeliveryAddress = new Address { Id = Guid.NewGuid(), Province = "d01", City = "d02", Area = "d03", Street = "d004", Other = "xxx2" },
                Products = new HashSet<Product>
                {
                    new Product{ Key=Guid.NewGuid(),Name="product 1", Price=20,Unit="件",Quantity=10 },
                    new Product{ Key=Guid.NewGuid(),Name="product 2", Price=10,Unit="件",Quantity=1 },
                },
                OrderStatus = OrderStatus.Created,
                PayStatus = PayStatus.NoPay,
                CreationTime = DateTime.Now,
            };

            await _db.AddAsync(order);
            await _db.SaveChangesAsync();
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public async Task Put(OrderId id, [FromBody] string value)
        {
            var order = await _db.Set<Order>().FindAsync(id) ?? throw new Exception("订单未找到");

            order.Seller.Name = value;

            await _db.SaveChangesAsync();
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            var order = await _db.Set<Order>().FindAsync(new OrderId(id)) ?? throw new Exception("订单未找到");

            _db.Remove(order);

            await _db.SaveChangesAsync();
        }
    }
}
