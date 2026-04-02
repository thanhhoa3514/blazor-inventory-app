using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierCustomerMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "StockReceipts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "StockIssues",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.Sql("""
                INSERT INTO [Suppliers] ([Name], [Description], [IsActive], [CreatedAtUtc], [LastUpdatedUtc])
                SELECT DISTINCT
                    LTRIM(RTRIM([Supplier])) AS [Name],
                    NULL AS [Description],
                    CAST(1 AS bit) AS [IsActive],
                    SYSUTCDATETIME() AS [CreatedAtUtc],
                    SYSUTCDATETIME() AS [LastUpdatedUtc]
                FROM [StockReceipts]
                WHERE [Supplier] IS NOT NULL
                  AND LTRIM(RTRIM([Supplier])) <> '';
                """);

            migrationBuilder.Sql("""
                INSERT INTO [Customers] ([Name], [Description], [IsActive], [CreatedAtUtc], [LastUpdatedUtc])
                SELECT DISTINCT
                    LTRIM(RTRIM([Customer])) AS [Name],
                    NULL AS [Description],
                    CAST(1 AS bit) AS [IsActive],
                    SYSUTCDATETIME() AS [CreatedAtUtc],
                    SYSUTCDATETIME() AS [LastUpdatedUtc]
                FROM [StockIssues]
                WHERE [Customer] IS NOT NULL
                  AND LTRIM(RTRIM([Customer])) <> '';
                """);

            migrationBuilder.Sql("""
                UPDATE sr
                SET sr.[SupplierId] = s.[Id]
                FROM [StockReceipts] sr
                INNER JOIN [Suppliers] s ON s.[Name] = LTRIM(RTRIM(sr.[Supplier]))
                WHERE sr.[Supplier] IS NOT NULL
                  AND LTRIM(RTRIM(sr.[Supplier])) <> '';
                """);

            migrationBuilder.Sql("""
                UPDATE si
                SET si.[CustomerId] = c.[Id]
                FROM [StockIssues] si
                INNER JOIN [Customers] c ON c.[Name] = LTRIM(RTRIM(si.[Customer]))
                WHERE si.[Customer] IS NOT NULL
                  AND LTRIM(RTRIM(si.[Customer])) <> '';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_StockReceipts_SupplierId",
                table: "StockReceipts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_StockIssues_CustomerId",
                table: "StockIssues",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Name",
                table: "Customers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Name",
                table: "Suppliers",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StockIssues_Customers_CustomerId",
                table: "StockIssues",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockReceipts_Suppliers_SupplierId",
                table: "StockReceipts",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "StockReceipts");

            migrationBuilder.DropColumn(
                name: "Customer",
                table: "StockIssues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "StockReceipts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Customer",
                table: "StockIssues",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE sr
                SET sr.[Supplier] = s.[Name]
                FROM [StockReceipts] sr
                INNER JOIN [Suppliers] s ON sr.[SupplierId] = s.[Id];
                """);

            migrationBuilder.Sql("""
                UPDATE si
                SET si.[Customer] = c.[Name]
                FROM [StockIssues] si
                INNER JOIN [Customers] c ON si.[CustomerId] = c.[Id];
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_StockIssues_Customers_CustomerId",
                table: "StockIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_StockReceipts_Suppliers_SupplierId",
                table: "StockReceipts");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_StockReceipts_SupplierId",
                table: "StockReceipts");

            migrationBuilder.DropIndex(
                name: "IX_StockIssues_CustomerId",
                table: "StockIssues");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "StockReceipts");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "StockIssues");
        }
    }
}
