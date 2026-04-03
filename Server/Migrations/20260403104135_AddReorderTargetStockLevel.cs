using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddReorderTargetStockLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetStockLevel",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "TargetStockLevel",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "TargetStockLevel",
                value: 25);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_TargetStockLevel_NonNegative",
                table: "Products",
                sql: "[TargetStockLevel] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_TargetStockLevel_NonNegative",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TargetStockLevel",
                table: "Products");
        }
    }
}
