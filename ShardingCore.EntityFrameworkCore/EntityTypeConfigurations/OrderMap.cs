using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShardingCore.Domain;

namespace ShardingCore.EntityFrameworkCore.EntityTypeConfigurations
{
    internal class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).IsRequired();

            builder.Property(o => o.OrderStatus).HasConversion<int>();
            builder.Property(o => o.PayStatus).HasConversion<int>();

            builder.OwnsOne(o => o.Buyer);
            builder.OwnsOne(o => o.Seller);
            builder.OwnsOne(o => o.ReceiverAddress);
            builder.OwnsOne(o => o.DeliveryAddress);
            builder.OwnsMany(o => o.Products, b =>
            {
                b.ToJson();
            });
        }
    }
}
