using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarehouseManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class addusertables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblRoles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblRoles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "TblUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TblUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_TblUserRoles_TblRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "TblRoles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblUserRoles_TblUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "TblUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TblRoles",
                columns: new[] { "RoleId", "Description", "RoleName" },
                values: new object[,]
                {
                    { 1, "Administrator", "Administrator" },
                    { 2, "Sales Manager", "SalesManager" },
                    { 3, "Warehouse Manager", "WarehouseManager" }
                });

            migrationBuilder.InsertData(
                table: "TblUsers",
                columns: new[] { "UserId", "EmailId", "IsActive", "UserName" },
                values: new object[,]
                {
                    { 1001, "Parveen.Rewadia@gmail.com", true, "Admin" },
                    { 1002, "ST_SouthDelhi@Gmail.com", true, "ST_SouthDelhi" },
                    { 1003, "WT_SouthDelhi@Gmail.com", true, "WT_SouthDelhi" }
                });

            migrationBuilder.InsertData(
                table: "TblUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 1001 },
                    { 2, 1002 },
                    { 3, 1003 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblUserRoles_RoleId",
                table: "TblUserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblUserRoles");

            migrationBuilder.DropTable(
                name: "TblRoles");

            migrationBuilder.DropTable(
                name: "TblUsers");
        }
    }
}
