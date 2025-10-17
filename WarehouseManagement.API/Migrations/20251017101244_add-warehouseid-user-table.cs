using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class addwarehouseidusertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "TblUsers",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TblUsers",
                keyColumn: "UserId",
                keyValue: 1001,
                column: "WarehouseId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TblUsers",
                keyColumn: "UserId",
                keyValue: 1002,
                column: "WarehouseId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TblUsers",
                keyColumn: "UserId",
                keyValue: 1003,
                column: "WarehouseId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_TblUsers_WarehouseId",
                table: "TblUsers",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblUsers_TblWarehouse_WarehouseId",
                table: "TblUsers",
                column: "WarehouseId",
                principalTable: "TblWarehouse",
                principalColumn: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblUsers_TblWarehouse_WarehouseId",
                table: "TblUsers");

            migrationBuilder.DropIndex(
                name: "IX_TblUsers_WarehouseId",
                table: "TblUsers");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "TblUsers");
        }
    }
}
