using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Domain;
using ShardingCore.EntityFrameworkCore;

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
        public async Task<Order?> Get(Guid id)
        {
            return await _db.Set<Order>().FirstOrDefaultAsync(w => w.Id == id);
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task Post([FromBody] string value)
        {
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

            await _db.AddAsync(order);
            await _db.SaveChangesAsync();
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
