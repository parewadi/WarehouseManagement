using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class addfielduserstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "TblUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "TblUsers",
                keyColumn: "UserId",
                keyValue: 1001,
                column: "Password",
                value: "Admin123");

            migrationBuilder.UpdateData(
                table: "TblUsers",
                keyColumn: "UserId",
                keyValue: 1002,
                column: "Password",
                value: "Sales123");

            migrationBuilder.UpdateData(
                table: "TblUsers",
                keyColumn: "UserId",
                keyValue: 1003,
                column: "Password",
                value: "Warehouse123");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "TblUsers");
        }
    }
}
