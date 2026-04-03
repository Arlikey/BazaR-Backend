using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "title",
                table: "seller_reviews");

            migrationBuilder.DropColumn(
                name: "title",
                table: "product_reviews");

            migrationBuilder.AddColumn<string>(
                name: "advantages",
                table: "seller_reviews",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "disadvantages",
                table: "seller_reviews",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "advantages",
                table: "product_reviews",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "disadvantages",
                table: "product_reviews",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "advantages",
                table: "seller_reviews");

            migrationBuilder.DropColumn(
                name: "disadvantages",
                table: "seller_reviews");

            migrationBuilder.DropColumn(
                name: "advantages",
                table: "product_reviews");

            migrationBuilder.DropColumn(
                name: "disadvantages",
                table: "product_reviews");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "seller_reviews",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "product_reviews",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
