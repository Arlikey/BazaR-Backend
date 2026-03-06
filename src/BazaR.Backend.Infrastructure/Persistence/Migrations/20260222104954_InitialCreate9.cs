using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "offers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "delivery_days",
                table: "offers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "min_order_quantity",
                table: "offers",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "old_price_amount",
                table: "offers",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "old_price_currency",
                table: "offers",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "seller_sku",
                table: "offers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "offers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "ix_offers_created_at",
                table: "offers",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_offers_status",
                table: "offers",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_offers_seller_id_seller_sku",
                table: "offers",
                columns: new[] { "seller_id", "seller_sku" },
                unique: true,
                filter: "seller_sku IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "ck_offers_delivery_days_non_negative",
                table: "offers",
                sql: "delivery_days IS NULL OR delivery_days >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_offers_min_order_quantity_positive",
                table: "offers",
                sql: "min_order_quantity >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "ck_offers_old_price_amount_non_negative",
                table: "offers",
                sql: "old_price_amount IS NULL OR old_price_amount >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_offers_old_price_currency_len",
                table: "offers",
                sql: "old_price_currency IS NULL OR char_length(old_price_currency) = 3");

            migrationBuilder.AddCheckConstraint(
                name: "ck_offers_old_price_nulls_together",
                table: "offers",
                sql: "(old_price_amount IS NULL AND old_price_currency IS NULL) OR (old_price_amount IS NOT NULL AND old_price_currency IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_offers_price_nulls_together",
                table: "offers",
                sql: "(price_amount IS NULL AND price_currency IS NULL) OR (price_amount IS NOT NULL AND price_currency IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_offers_created_at",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "ix_offers_status",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "ux_offers_seller_id_seller_sku",
                table: "offers");

            migrationBuilder.DropCheckConstraint(
                name: "ck_offers_delivery_days_non_negative",
                table: "offers");

            migrationBuilder.DropCheckConstraint(
                name: "ck_offers_min_order_quantity_positive",
                table: "offers");

            migrationBuilder.DropCheckConstraint(
                name: "ck_offers_old_price_amount_non_negative",
                table: "offers");

            migrationBuilder.DropCheckConstraint(
                name: "ck_offers_old_price_currency_len",
                table: "offers");

            migrationBuilder.DropCheckConstraint(
                name: "ck_offers_old_price_nulls_together",
                table: "offers");

            migrationBuilder.DropCheckConstraint(
                name: "ck_offers_price_nulls_together",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "delivery_days",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "min_order_quantity",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "old_price_amount",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "old_price_currency",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "seller_sku",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "offers");
        }
    }
}
