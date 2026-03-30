using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "destination_nova_post_recipient_division_id",
                table: "shippings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "destination_nova_post_recipient_division_name",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sender_country_code",
                table: "shippings",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sender_email",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sender_name",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sender_nova_post_division_id",
                table: "shippings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sender_nova_post_division_name",
                table: "shippings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sender_phone",
                table: "shippings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "shipping_country_code",
                table: "sellers",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shipping_nova_post_division_id",
                table: "sellers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shipping_nova_post_division_name",
                table: "sellers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shipping_sender_email",
                table: "sellers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shipping_sender_name",
                table: "sellers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shipping_sender_phone",
                table: "sellers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shipping_nova_post_recipient_division_id",
                table: "checkout_lines",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shipping_nova_post_recipient_division_name",
                table: "checkout_lines",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "shipping_parcels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    insurance_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    row_number = table.Column<int>(type: "integer", nullable: false),
                    width = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    length = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    height = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    actual_weight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    volumetric_weight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    cargo_category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "parcel"),
                    shipping_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipping_parcels", x => x.id);
                    table.ForeignKey(
                        name: "FK_shipping_parcels_shippings_shipping_id",
                        column: x => x.shipping_id,
                        principalTable: "shippings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_shipping_parcels_shipping_id_row_number",
                table: "shipping_parcels",
                columns: new[] { "shipping_id", "row_number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shipping_parcels");

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
                name: "destination_nova_post_recipient_division_id",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "destination_nova_post_recipient_division_name",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "sender_country_code",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "sender_email",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "sender_name",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "sender_nova_post_division_id",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "sender_nova_post_division_name",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "sender_phone",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "shipping_country_code",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "shipping_nova_post_division_id",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "shipping_nova_post_division_name",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "shipping_sender_email",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "shipping_sender_name",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "shipping_sender_phone",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "shipping_nova_post_recipient_division_id",
                table: "checkout_lines");

            migrationBuilder.DropColumn(
                name: "shipping_nova_post_recipient_division_name",
                table: "checkout_lines");
        }
    }
}
