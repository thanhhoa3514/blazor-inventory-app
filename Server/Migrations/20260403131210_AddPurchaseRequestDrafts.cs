using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseRequestDrafts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseRequestDrafts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DraftNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequestDrafts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequestDraftLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestDraftId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    SuggestedQty = table.Column<int>(type: "int", nullable: false),
                    RequestedQty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequestDraftLines", x => x.Id);
                    table.CheckConstraint("CK_PurchaseRequestDraftLines_RequestedQty_Positive", "[RequestedQty] > 0");
                    table.CheckConstraint("CK_PurchaseRequestDraftLines_SuggestedQty_NonNegative", "[SuggestedQty] >= 0");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestDraftLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestDraftLines_PurchaseRequestDrafts_PurchaseRequestDraftId",
                        column: x => x.PurchaseRequestDraftId,
                        principalTable: "PurchaseRequestDrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestDraftLines_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestDraftLines_ProductId",
                table: "PurchaseRequestDraftLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestDraftLines_PurchaseRequestDraftId",
                table: "PurchaseRequestDraftLines",
                column: "PurchaseRequestDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestDraftLines_SupplierId",
                table: "PurchaseRequestDraftLines",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestDrafts_DraftNo",
                table: "PurchaseRequestDrafts",
                column: "DraftNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseRequestDraftLines");

            migrationBuilder.DropTable(
                name: "PurchaseRequestDrafts");
        }
    }
}
