using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payment_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    bank_recipient_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    bank_iban = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    bank_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    bank_tax_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    bank_swift = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    bank_purpose_template = table.Column<string>(type: "text", nullable: true),
                    liqpay_public_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    liqpay_private_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    liqpay_result_url = table.Column<string>(type: "text", nullable: true),
                    liqpay_server_callback_url = table.Column<string>(type: "text", nullable: true),
                    liqpay_checkout_enabled = table.Column<bool>(type: "boolean", nullable: true),
                    liqpay_privatpay_enabled = table.Column<bool>(type: "boolean", nullable: true),
                    liqpay_installments_enabled = table.Column<bool>(type: "boolean", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_profile_methods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    method_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    requires_online_authorization = table.Column<bool>(type: "boolean", nullable: false),
                    requires_bank_account = table.Column<bool>(type: "boolean", nullable: false),
                    requires_liqpay = table.Column<bool>(type: "boolean", nullable: false),
                    min_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    max_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    payment_profile_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_profile_methods", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_profile_methods_payment_profiles_payment_profile_id",
                        column: x => x.payment_profile_id,
                        principalTable: "payment_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_payment_profile_methods_method_type",
                table: "payment_profile_methods",
                column: "method_type");

            migrationBuilder.CreateIndex(
                name: "ix_payment_profile_methods_profile_id",
                table: "payment_profile_methods",
                column: "payment_profile_id");

            migrationBuilder.CreateIndex(
                name: "ux_payment_profile_methods_profile_id_method_type",
                table: "payment_profile_methods",
                columns: new[] { "payment_profile_id", "method_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_payment_profiles_created_at_utc",
                table: "payment_profiles",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_payment_profiles_seller_id",
                table: "payment_profiles",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_profiles_status",
                table: "payment_profiles",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment_profile_methods");

            migrationBuilder.DropTable(
                name: "payment_profiles");
        }
    }
}
