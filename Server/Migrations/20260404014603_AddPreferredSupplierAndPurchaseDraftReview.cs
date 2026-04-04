using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPreferredSupplierAndPurchaseDraftReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAtUtc",
                table: "PurchaseRequestDrafts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByUserId",
                table: "PurchaseRequestDrafts",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByUserName",
                table: "PurchaseRequestDrafts",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreferredSupplierId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "PreferredSupplierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "PreferredSupplierId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Products_PreferredSupplierId",
                table: "Products",
                column: "PreferredSupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers_PreferredSupplierId",
                table: "Products",
                column: "PreferredSupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_PreferredSupplierId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_PreferredSupplierId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ReviewedAtUtc",
                table: "PurchaseRequestDrafts");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "PurchaseRequestDrafts");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserName",
                table: "PurchaseRequestDrafts");

            migrationBuilder.DropColumn(
                name: "PreferredSupplierId",
                table: "Products");
        }
    }
}
