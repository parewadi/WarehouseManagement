using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class addIsRevertedcoltransfertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReverted",
                table: "TblTransfer",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReverted",
                table: "TblTransfer");
        }
    }
}
