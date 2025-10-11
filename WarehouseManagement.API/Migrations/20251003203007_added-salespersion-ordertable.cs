using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class addedsalespersionordertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "TblOrder",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesPersonId",
                table: "TblOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TblOrder_SalesPersonId",
                table: "TblOrder",
                column: "SalesPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblOrder_TblUsers_SalesPersonId",
                table: "TblOrder",
                column: "SalesPersonId",
                principalTable: "TblUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblOrder_TblUsers_SalesPersonId",
                table: "TblOrder");

            migrationBuilder.DropIndex(
                name: "IX_TblOrder_SalesPersonId",
                table: "TblOrder");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "TblOrder");

            migrationBuilder.DropColumn(
                name: "SalesPersonId",
                table: "TblOrder");
        }
    }
}
