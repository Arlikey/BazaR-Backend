using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "destination_nova_post_recipient_division_id",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "destination_nova_post_recipient_division_name",
                table: "shippings");

            migrationBuilder.DropColumn(
                name: "shipping_nova_post_recipient_division_id",
                table: "checkout_lines");

            migrationBuilder.DropColumn(
                name: "shipping_nova_post_recipient_division_name",
                table: "checkout_lines");

            migrationBuilder.RenameColumn(
                name: "sender_nova_post_division_name",
                table: "shippings",
                newName: "sender_pickup_point_name");

            migrationBuilder.RenameColumn(
                name: "sender_nova_post_division_id",
                table: "shippings",
                newName: "sender_pickup_point_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "sender_pickup_point_name",
                table: "shippings",
                newName: "sender_nova_post_division_name");

            migrationBuilder.RenameColumn(
                name: "sender_pickup_point_code",
                table: "shippings",
                newName: "sender_nova_post_division_id");

            migrationBuilder.AddColumn<string>(
                name: "destination_nova_post_recipient_division_id",
                table: "shippings",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "destination_nova_post_recipient_division_name",
                table: "shippings",
                type: "character varying(256)",
                maxLength: 256,
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
        }
    }
}
