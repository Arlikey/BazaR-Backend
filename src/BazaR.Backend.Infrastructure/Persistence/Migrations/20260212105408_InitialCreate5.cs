using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_products_slug",
                table: "products");

            migrationBuilder.DropIndex(
                name: "ux_products_vendor_code",
                table: "products");

            migrationBuilder.DropCheckConstraint(
                name: "ck_products_status_valid",
                table: "products");

            migrationBuilder.AddColumn<string>(
                name: "barcode",
                table: "products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "hidden_at",
                table: "products",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "hidden_by",
                table: "products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hidden_reason",
                table: "products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "owner_seller_id",
                table: "products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "sellers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    legal_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    tax_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    country_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    support_email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    support_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    last_decision_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_decision_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_rejection_reason = table.Column<string>(type: "text", nullable: true),
                    suspension_reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sellers", x => x.id);
                    table.CheckConstraint("ck_sellers_name_not_empty", "char_length(name) > 0");
                    table.CheckConstraint("ck_sellers_slug_not_empty", "char_length(slug) > 0");
                });

            migrationBuilder.CreateIndex(
                name: "ix_products_barcode",
                table: "products",
                column: "barcode");

            migrationBuilder.CreateIndex(
                name: "ix_products_owner_seller_id",
                table: "products",
                column: "owner_seller_id");

            migrationBuilder.CreateIndex(
                name: "ux_products_owner_slug",
                table: "products",
                columns: new[] { "owner_seller_id", "slug" },
                unique: true,
                filter: "slug IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_products_owner_vendor_code",
                table: "products",
                columns: new[] { "owner_seller_id", "vendor_code" },
                unique: true,
                filter: "vendor_code IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "ck_products_hidden_fields",
                table: "products",
                sql: "(status <> 3) OR (hidden_at IS NOT NULL AND hidden_by IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_products_status_valid",
                table: "products",
                sql: "status IN (0,1,2,3)");

            migrationBuilder.CreateIndex(
                name: "ix_sellers_owner_user_id",
                table: "sellers",
                column: "owner_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_sellers_status",
                table: "sellers",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_sellers_slug",
                table: "sellers",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_sellers_tax_number",
                table: "sellers",
                column: "tax_number",
                unique: true,
                filter: "tax_number IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sellers");

            migrationBuilder.DropIndex(
                name: "ix_products_barcode",
                table: "products");

            migrationBuilder.DropIndex(
                name: "ix_products_owner_seller_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "ux_products_owner_slug",
                table: "products");

            migrationBuilder.DropIndex(
                name: "ux_products_owner_vendor_code",
                table: "products");

            migrationBuilder.DropCheckConstraint(
                name: "ck_products_hidden_fields",
                table: "products");

            migrationBuilder.DropCheckConstraint(
                name: "ck_products_status_valid",
                table: "products");

            migrationBuilder.DropColumn(
                name: "barcode",
                table: "products");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "products");

            migrationBuilder.DropColumn(
                name: "hidden_at",
                table: "products");

            migrationBuilder.DropColumn(
                name: "hidden_by",
                table: "products");

            migrationBuilder.DropColumn(
                name: "hidden_reason",
                table: "products");

            migrationBuilder.DropColumn(
                name: "owner_seller_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "products");

            migrationBuilder.CreateIndex(
                name: "ux_products_slug",
                table: "products",
                column: "slug",
                unique: true,
                filter: "slug IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_products_vendor_code",
                table: "products",
                column: "vendor_code",
                unique: true,
                filter: "vendor_code IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "ck_products_status_valid",
                table: "products",
                sql: "status IN (0,1,2)");
        }
    }
}
