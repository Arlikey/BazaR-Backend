using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_carts_user_id",
                table: "carts");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "cart_items",
                newName: "offer_id");

            migrationBuilder.RenameIndex(
                name: "IX_cart_items_cart_id_product_id",
                table: "cart_items",
                newName: "IX_cart_items_cart_id_offer_id");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "carts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "carts",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_activity_at",
                table: "carts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "carts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "carts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<decimal>(
                name: "price_amount",
                table: "cart_items",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "added_at",
                table: "cart_items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "cart_items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_carts_last_activity_at",
                table: "carts",
                column: "last_activity_at");

            migrationBuilder.CreateIndex(
                name: "IX_carts_updated_at",
                table: "carts",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_carts_user_id",
                table: "carts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_cart_id",
                table: "cart_items",
                column: "cart_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_carts_last_activity_at",
                table: "carts");

            migrationBuilder.DropIndex(
                name: "IX_carts_updated_at",
                table: "carts");

            migrationBuilder.DropIndex(
                name: "IX_carts_user_id",
                table: "carts");

            migrationBuilder.DropIndex(
                name: "IX_cart_items_cart_id",
                table: "cart_items");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "carts");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "carts");

            migrationBuilder.DropColumn(
                name: "last_activity_at",
                table: "carts");

            migrationBuilder.DropColumn(
                name: "status",
                table: "carts");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "carts");

            migrationBuilder.DropColumn(
                name: "added_at",
                table: "cart_items");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "cart_items");

            migrationBuilder.RenameColumn(
                name: "offer_id",
                table: "cart_items",
                newName: "product_id");

            migrationBuilder.RenameIndex(
                name: "IX_cart_items_cart_id_offer_id",
                table: "cart_items",
                newName: "IX_cart_items_cart_id_product_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "price_amount",
                table: "cart_items",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_carts_user_id",
                table: "carts",
                column: "user_id",
                unique: true);
        }
    }
}
