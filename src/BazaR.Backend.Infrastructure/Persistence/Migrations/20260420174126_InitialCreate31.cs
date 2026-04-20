using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "viewed_products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ViewedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_viewed_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_viewed_products_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_viewed_products_ProductId",
                table: "viewed_products",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_viewed_products_UserId_ProductId",
                table: "viewed_products",
                columns: new[] { "UserId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_viewed_products_UserId_ViewedAtUtc",
                table: "viewed_products",
                columns: new[] { "UserId", "ViewedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "viewed_products");
        }
    }
}
