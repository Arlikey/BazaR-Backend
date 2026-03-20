using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "actual_liqpay_pay_type",
                table: "payments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "callback_data",
                table: "payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "callback_received_at_utc",
                table: "payments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "callback_signature",
                table: "payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "card_bank",
                table: "payments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "card_mask",
                table: "payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "card_type",
                table: "payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "external_transaction_id",
                table: "payments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_provider_sync_at_utc",
                table: "payments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "provider_amount",
                table: "payments",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "provider_currency",
                table: "payments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "requested_liqpay_pay_type",
                table: "payments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "seller_id",
                table: "payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_payments_external_transaction_id",
                table: "payments",
                column: "external_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_seller_id",
                table: "payments",
                column: "seller_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_payments_external_transaction_id",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "ix_payments_seller_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "actual_liqpay_pay_type",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "callback_data",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "callback_received_at_utc",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "callback_signature",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "card_bank",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "card_mask",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "card_type",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "external_transaction_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "last_provider_sync_at_utc",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "provider_amount",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "provider_currency",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "requested_liqpay_pay_type",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "seller_id",
                table: "payments");
        }
    }
}
