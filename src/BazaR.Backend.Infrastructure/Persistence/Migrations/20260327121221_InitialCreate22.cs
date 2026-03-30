using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_shippings_created_at_utc",
                table: "shippings");

            migrationBuilder.DropIndex(
                name: "ix_shippings_delivered_at_utc",
                table: "shippings");

            migrationBuilder.DropIndex(
                name: "ix_shippings_method_type",
                table: "shippings");

            migrationBuilder.DropIndex(
                name: "ix_shippings_shipped_at_utc",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "ArrivedAtPickupPointAtUtc",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "CashOnDeliveryAmount",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "ExternalShipmentId",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "ExternalStatusCode",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "ExternalStatusName",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "ExternalStatusUpdatedAtUtc",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "TrackingUrl",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "carrier",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "cash_on_delivery_allowed",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "comment",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "cost_amount_currency",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "cost_amount_value",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "method_type",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "preparing_at_utc",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "ready_to_ship_at_utc",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "sender_email",
                table: "shippings");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "shippings",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "shipped_at_utc",
                table: "shippings",
                newName: "ready_for_pickup_at_utc");

            migrationBuilder.RenameColumn(
                name: "returned_at_utc",
                table: "shippings",
                newName: "dispatched_at_utc");

            migrationBuilder.RenameIndex(
                name: "ix_shippings_tracking_number",
                table: "shippings",
                newName: "IX_shippings_tracking_number");

            migrationBuilder.RenameIndex(
                name: "ix_shippings_status",
                table: "shippings",
                newName: "IX_shippings_status");

            migrationBuilder.RenameIndex(
                name: "ix_shippings_seller_id",
                table: "shippings",
                newName: "IX_shippings_seller_id");

            migrationBuilder.RenameIndex(
                name: "ix_shippings_order_id",
                table: "shippings",
                newName: "IX_shippings_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_shippings_user_id",
                table: "shippings",
                newName: "IX_shippings_customer_id");

            migrationBuilder.RenameIndex(
                name: "ux_shipping_parcels_shipping_id_row_number",
                table: "shipping_parcels",
                newName: "IX_shipping_parcels_shipping_id_row_number");

            migrationBuilder.AlterColumn<string>(
                name: "tracking_number",
                table: "shippings",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "shippings",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "settlement_mode",
                table: "shippings",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "sender_phone",
                table: "shippings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "sender_nova_post_division_name",
                table: "shippings",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "sender_nova_post_division_id",
                table: "shippings",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "sender_name",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "sender_country_code",
                table: "shippings",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "recipient_email",
                table: "shippings",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_street",
                table: "shippings",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_postal_code",
                table: "shippings",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_pickup_point_name",
                table: "shippings",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_pickup_point_code",
                table: "shippings",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_nova_post_recipient_division_name",
                table: "shippings",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_nova_post_recipient_division_id",
                table: "shippings",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_city",
                table: "shippings",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "method",
                table: "shippings",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "cargo_category",
                table: "shipping_parcels",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "parcel");

            migrationBuilder.CreateIndex(
                name: "IX_shippings_method",
                table: "shippings",
                column: "method");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_shippings_method",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "method",
                table: "shippings");

            migrationBuilder.RenameColumn(
                name: "ready_for_pickup_at_utc",
                table: "shippings",
                newName: "shipped_at_utc");

            migrationBuilder.RenameColumn(
                name: "dispatched_at_utc",
                table: "shippings",
                newName: "returned_at_utc");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "shippings",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_shippings_tracking_number",
                table: "shippings",
                newName: "ix_shippings_tracking_number");

            migrationBuilder.RenameIndex(
                name: "IX_shippings_status",
                table: "shippings",
                newName: "ix_shippings_status");

            migrationBuilder.RenameIndex(
                name: "IX_shippings_seller_id",
                table: "shippings",
                newName: "ix_shippings_seller_id");

            migrationBuilder.RenameIndex(
                name: "IX_shippings_order_id",
                table: "shippings",
                newName: "ix_shippings_order_id");

            migrationBuilder.RenameIndex(
                name: "IX_shippings_customer_id",
                table: "shippings",
                newName: "ix_shippings_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_shipping_parcels_shipping_id_row_number",
                table: "shipping_parcels",
                newName: "ux_shipping_parcels_shipping_id_row_number");

            migrationBuilder.AlterColumn<string>(
                name: "tracking_number",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "shippings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "settlement_mode",
                table: "shippings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "sender_phone",
                table: "shippings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "sender_nova_post_division_name",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "sender_nova_post_division_id",
                table: "shippings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "sender_name",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "sender_country_code",
                table: "shippings",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "recipient_email",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_street",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_postal_code",
                table: "shippings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_pickup_point_name",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_pickup_point_code",
                table: "shippings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_nova_post_recipient_division_name",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_nova_post_recipient_division_id",
                table: "shippings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "destination_city",
                table: "shippings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ArrivedAtPickupPointAtUtc",
                table: "shippings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CashOnDeliveryAmount",
                table: "shippings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalShipmentId",
                table: "shippings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalStatusCode",
                table: "shippings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalStatusName",
                table: "shippings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExternalStatusUpdatedAtUtc",
                table: "shippings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingUrl",
                table: "shippings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "carrier",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "cash_on_delivery_allowed",
                table: "shippings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "shippings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cost_amount_currency",
                table: "shippings",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "cost_amount_value",
                table: "shippings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "method_type",
                table: "shippings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "preparing_at_utc",
                table: "shippings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ready_to_ship_at_utc",
                table: "shippings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sender_email",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cargo_category",
                table: "shipping_parcels",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "parcel",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.CreateIndex(
                name: "ix_shippings_created_at_utc",
                table: "shippings",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_shippings_delivered_at_utc",
                table: "shippings",
                column: "delivered_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_shippings_method_type",
                table: "shippings",
                column: "method_type");

            migrationBuilder.CreateIndex(
                name: "ix_shippings_shipped_at_utc",
                table: "shippings",
                column: "shipped_at_utc");
        }
    }
}
