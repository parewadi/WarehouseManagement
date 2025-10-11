using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class migration1addtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblProduct",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblProduct", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "TblWarehouse",
                columns: table => new
                {
                    WarehouseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Pincode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblWarehouse", x => x.WarehouseId);
                });

            migrationBuilder.CreateTable(
                name: "TblInventory",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblInventory", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_TblInventory_TblProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "TblProduct",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblInventory_TblWarehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "TblWarehouse",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblOrder",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedWarehouseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblOrder", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_TblOrder_TblWarehouse_AssignedWarehouseId",
                        column: x => x.AssignedWarehouseId,
                        principalTable: "TblWarehouse",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblItem",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuantityRequested = table.Column<int>(type: "int", nullable: false),
                    QuantityServed = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblItem", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_TblItem_TblOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "TblOrder",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblItem_TblProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "TblProduct",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblTransfer",
                columns: table => new
                {
                    TransferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    FromWarehouseId = table.Column<int>(type: "int", nullable: false),
                    ToWarehouseId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTransfer", x => x.TransferId);
                    table.ForeignKey(
                        name: "FK_TblTransfer_TblOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "TblOrder",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblTransfer_TblProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "TblProduct",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTransfer_TblWarehouse_FromWarehouseId",
                        column: x => x.FromWarehouseId,
                        principalTable: "TblWarehouse",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblTransfer_TblWarehouse_ToWarehouseId",
                        column: x => x.ToWarehouseId,
                        principalTable: "TblWarehouse",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblInventory_ProductId",
                table: "TblInventory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TblInventory_WarehouseId",
                table: "TblInventory",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_TblItem_OrderId",
                table: "TblItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TblItem_ProductId",
                table: "TblItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TblOrder_AssignedWarehouseId",
                table: "TblOrder",
                column: "AssignedWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTransfer_FromWarehouseId",
                table: "TblTransfer",
                column: "FromWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTransfer_OrderId",
                table: "TblTransfer",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTransfer_ProductId",
                table: "TblTransfer",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTransfer_ToWarehouseId",
                table: "TblTransfer",
                column: "ToWarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblInventory");

            migrationBuilder.DropTable(
                name: "TblItem");

            migrationBuilder.DropTable(
                name: "TblTransfer");

            migrationBuilder.DropTable(
                name: "TblOrder");

            migrationBuilder.DropTable(
                name: "TblProduct");

            migrationBuilder.DropTable(
                name: "TblWarehouse");
        }
    }
}
