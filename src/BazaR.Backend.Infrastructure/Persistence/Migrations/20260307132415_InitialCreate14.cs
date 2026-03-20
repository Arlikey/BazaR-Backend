using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "orders",
                newName: "customer_user_id");

            migrationBuilder.RenameColumn(
                name: "street",
                table: "orders",
                newName: "delivery_street");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "orders",
                newName: "delivery_country");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "orders",
                newName: "delivery_city");

            migrationBuilder.RenameColumn(
                name: "apartment",
                table: "orders",
                newName: "delivery_apartment");

            migrationBuilder.AlterColumn<string>(
                name: "delivery_street",
                table: "orders",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_city",
                table: "orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "cancellation_reason",
                table: "orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cancelled_at_utc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "completed_at_utc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at_utc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "customer_comment",
                table: "orders",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_email",
                table: "orders",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "customer_first_name",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "customer_last_name",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "customer_phone",
                table: "orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "delivered_at_utc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "paid_at_utc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at_utc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<decimal>(
                name: "price_amount",
                table: "order_items",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<int>(
                name: "cancelled_quantity",
                table: "order_items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "order_items",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "product_name",
                table: "order_items",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sku",
                table: "order_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_orders_created_at_utc",
                table: "orders",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_orders_status",
                table: "orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_order_items_order_id",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_items_product_id",
                table: "order_items",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_orders_created_at_utc",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_orders_status",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_order_items_order_id",
                table: "order_items");

            migrationBuilder.DropIndex(
                name: "ix_order_items_product_id",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "cancellation_reason",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "cancelled_at_utc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "completed_at_utc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "created_at_utc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "customer_comment",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "customer_email",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "customer_first_name",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "customer_last_name",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "customer_phone",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "delivered_at_utc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "paid_at_utc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "updated_at_utc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "cancelled_quantity",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "product_name",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "sku",
                table: "order_items");

            migrationBuilder.RenameColumn(
                name: "delivery_street",
                table: "orders",
                newName: "street");

            migrationBuilder.RenameColumn(
                name: "delivery_country",
                table: "orders",
                newName: "country");

            migrationBuilder.RenameColumn(
                name: "delivery_city",
                table: "orders",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "delivery_apartment",
                table: "orders",
                newName: "apartment");

            migrationBuilder.RenameColumn(
                name: "customer_user_id",
                table: "orders",
                newName: "user_id");

            migrationBuilder.AlterColumn<string>(
                name: "street",
                table: "orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<decimal>(
                name: "price_amount",
                table: "order_items",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "order_items",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order_id_product_id",
                table: "order_items",
                columns: new[] { "order_id", "product_id" });
        }
    }
}
