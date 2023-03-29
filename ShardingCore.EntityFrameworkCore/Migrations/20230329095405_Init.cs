using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShardingCore.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Buyer_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Buyer_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryAddress_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeliveryAddress_Province = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress_Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress_Other = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    PayStatus = table.Column<int>(type: "int", nullable: false),
                    ReceiverAddress_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverAddress_Province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverAddress_Area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverAddress_Other = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seller_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Seller_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Products = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order");
        }
    }
}
