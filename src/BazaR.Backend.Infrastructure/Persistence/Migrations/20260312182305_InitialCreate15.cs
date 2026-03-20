using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customer_user_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "delivery_country",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "order_items");

            migrationBuilder.RenameColumn(
                name: "price_currency",
                table: "order_items",
                newName: "price_snapshot_currency");

            migrationBuilder.RenameColumn(
                name: "price_amount",
                table: "order_items",
                newName: "price_snapshot_value");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "delivery_street",
                table: "orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_city",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "customer_phone",
                table: "orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "customer_email",
                table: "orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "customer_comment",
                table: "orders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cancellation_reason",
                table: "orders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "buyer_user_id",
                table: "orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "delivery_building",
                table: "orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "delivery_method",
                table: "orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "delivery_postal_code",
                table: "orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "delivery_region",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "delivery_warehouse",
                table: "orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "price_snapshot_currency",
                table: "order_items",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.CreateTable(
                name: "checkouts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cart_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    items_subtotal_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    items_subtotal_currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shipping_total_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    shipping_total_currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    grand_total_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    grand_total_currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    submitted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkouts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    amount_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    amount_currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    refunded_amount_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    refunded_amount_currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    merchant_order_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    external_payment_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    external_order_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    external_session_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    external_status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    checkout_action_url = table.Column<string>(type: "text", nullable: true),
                    checkout_data = table.Column<string>(type: "text", nullable: true),
                    checkout_signature = table.Column<string>(type: "text", nullable: true),
                    failure_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    failure_message = table.Column<string>(type: "text", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    checkout_started_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    authorized_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    paid_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    failed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    refunded_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shipping_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipping_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shippings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    method_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    recipient_first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    recipient_last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    recipient_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    recipient_email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    destination_country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    destination_region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    destination_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    destination_street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    destination_house = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    destination_apartment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    destination_postal_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    destination_pickup_point_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    destination_pickup_point_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cost_amount_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    cost_amount_currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    cash_on_delivery_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    carrier = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tracking_number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    preparing_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ready_to_ship_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    shipped_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    delivered_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    returned_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shippings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "checkout_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    checkout_id = table.Column<Guid>(type: "uuid", nullable: false),
                    offer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    unit_price_currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    recipient_first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    recipient_last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    recipient_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    recipient_email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    recipient_is_customer_recipient = table.Column<bool>(type: "boolean", nullable: true),
                    shipping_method_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    shipping_country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    shipping_region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    shipping_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    shipping_street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    shipping_house = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    shipping_apartment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    shipping_postal_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    shipping_pickup_point_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    shipping_pickup_point_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    shipping_comment = table.Column<string>(type: "text", nullable: true),
                    shipping_cost_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    shipping_cost_currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    payment_method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    payment_provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    payment_requires_online_authorization = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkout_lines", x => new { x.checkout_id, x.id });
                    table.ForeignKey(
                        name: "FK_checkout_lines_checkouts_checkout_id",
                        column: x => x.checkout_id,
                        principalTable: "checkouts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shipping_profile_methods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    method_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    base_fee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    free_shipping_from_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    allow_cash_on_delivery = table.Column<bool>(type: "boolean", nullable: false),
                    requires_city = table.Column<bool>(type: "boolean", nullable: false),
                    requires_pickup_point = table.Column<bool>(type: "boolean", nullable: false),
                    requires_street_address = table.Column<bool>(type: "boolean", nullable: false),
                    estimated_days_min = table.Column<int>(type: "integer", nullable: true),
                    estimated_days_max = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    shipping_profile_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipping_profile_methods", x => x.id);
                    table.ForeignKey(
                        name: "FK_shipping_profile_methods_shipping_profiles_shipping_profile~",
                        column: x => x.shipping_profile_id,
                        principalTable: "shipping_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_orders_buyer_user_id",
                table: "orders",
                column: "buyer_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_completed_at_utc",
                table: "orders",
                column: "completed_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_orders_delivered_at_utc",
                table: "orders",
                column: "delivered_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_orders_paid_at_utc",
                table: "orders",
                column: "paid_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_order_items_order_id_product_id",
                table: "order_items",
                columns: new[] { "order_id", "product_id" });

            migrationBuilder.CreateIndex(
                name: "ix_checkout_lines_checkout_id",
                table: "checkout_lines",
                column: "checkout_id");

            migrationBuilder.CreateIndex(
                name: "ix_checkout_lines_offer_id",
                table: "checkout_lines",
                column: "offer_id");

            migrationBuilder.CreateIndex(
                name: "ix_checkout_lines_product_id",
                table: "checkout_lines",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_checkout_lines_seller_id",
                table: "checkout_lines",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "ix_checkouts_cart_id",
                table: "checkouts",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "ix_checkouts_created_at_utc",
                table: "checkouts",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_checkouts_status",
                table: "checkouts",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_checkouts_submitted_at_utc",
                table: "checkouts",
                column: "submitted_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_checkouts_user_id",
                table: "checkouts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_created_at_utc",
                table: "payments",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_payments_external_order_reference",
                table: "payments",
                column: "external_order_reference");

            migrationBuilder.CreateIndex(
                name: "ix_payments_external_payment_id",
                table: "payments",
                column: "external_payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_external_session_id",
                table: "payments",
                column: "external_session_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_method",
                table: "payments",
                column: "method");

            migrationBuilder.CreateIndex(
                name: "ix_payments_order_id",
                table: "payments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_paid_at_utc",
                table: "payments",
                column: "paid_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_payments_provider",
                table: "payments",
                column: "provider");

            migrationBuilder.CreateIndex(
                name: "ix_payments_status",
                table: "payments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_payments_user_id",
                table: "payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_payments_merchant_order_reference",
                table: "payments",
                column: "merchant_order_reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shipping_profile_methods_method_type",
                table: "shipping_profile_methods",
                column: "method_type");

            migrationBuilder.CreateIndex(
                name: "ix_shipping_profile_methods_profile_id",
                table: "shipping_profile_methods",
                column: "shipping_profile_id");

            migrationBuilder.CreateIndex(
                name: "ux_shipping_profile_methods_profile_id_method_type",
                table: "shipping_profile_methods",
                columns: new[] { "shipping_profile_id", "method_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shipping_profiles_seller_id",
                table: "shipping_profiles",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "ix_shipping_profiles_status",
                table: "shipping_profiles",
                column: "status");

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
                name: "ix_shippings_order_id",
                table: "shippings",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_shippings_seller_id",
                table: "shippings",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "ix_shippings_shipped_at_utc",
                table: "shippings",
                column: "shipped_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_shippings_status",
                table: "shippings",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_shippings_tracking_number",
                table: "shippings",
                column: "tracking_number");

            migrationBuilder.CreateIndex(
                name: "ix_shippings_user_id",
                table: "shippings",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checkout_lines");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "shipping_profile_methods");

            migrationBuilder.DropTable(
                name: "shippings");

            migrationBuilder.DropTable(
                name: "checkouts");

            migrationBuilder.DropTable(
                name: "shipping_profiles");

            migrationBuilder.DropIndex(
                name: "ix_orders_buyer_user_id",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_orders_completed_at_utc",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_orders_delivered_at_utc",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_orders_paid_at_utc",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_order_items_order_id_product_id",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "buyer_user_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "delivery_building",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "delivery_method",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "delivery_postal_code",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "delivery_region",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "delivery_warehouse",
                table: "orders");

            migrationBuilder.RenameColumn(
                name: "price_snapshot_value",
                table: "order_items",
                newName: "price_amount");

            migrationBuilder.RenameColumn(
                name: "price_snapshot_currency",
                table: "order_items",
                newName: "price_currency");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_street",
                table: "orders",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_city",
                table: "orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "customer_phone",
                table: "orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "customer_email",
                table: "orders",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "customer_comment",
                table: "orders",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cancellation_reason",
                table: "orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "customer_user_id",
                table: "orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "delivery_country",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "price_currency",
                table: "order_items",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "order_items",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
