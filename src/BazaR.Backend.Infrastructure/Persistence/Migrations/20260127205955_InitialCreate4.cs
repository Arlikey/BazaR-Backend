using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attribute_definitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    value_type = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_system = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attribute_definitions", x => x.Id);
                    table.CheckConstraint("ck_attribute_definitions_code_not_empty", "char_length(code) > 0");
                    table.CheckConstraint("ck_attribute_definitions_name_not_empty", "char_length(name) > 0");
                });

            migrationBuilder.CreateTable(
                name: "category_attributes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    attribute_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    is_filterable = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    section_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    section_order = table.Column<int>(type: "integer", nullable: true),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category_attributes", x => x.id);
                    table.CheckConstraint("ck_category_attributes_section_order_non_negative", "section_order IS NULL OR section_order >= 0");
                    table.CheckConstraint("ck_category_attributes_sort_order_non_negative", "sort_order >= 0");
                    table.ForeignKey(
                        name: "FK_category_attributes_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_attribute_values",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    attribute_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text_value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    number_value = table.Column<decimal>(type: "numeric", nullable: true),
                    bool_value = table.Column<bool>(type: "boolean", nullable: true),
                    option_id = table.Column<Guid>(type: "uuid", nullable: true),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_attribute_values", x => x.id);
                    table.ForeignKey(
                        name: "FK_product_attribute_values_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attribute_options",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    attribute_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attribute_options", x => new { x.attribute_definition_id, x.Id });
                    table.CheckConstraint("ck_attribute_options_sort_order_non_negative", "sort_order >= 0");
                    table.CheckConstraint("ck_attribute_options_value_not_empty", "char_length(value) > 0");
                    table.ForeignKey(
                        name: "FK_attribute_options_attribute_definitions_attribute_definitio~",
                        column: x => x.attribute_definition_id,
                        principalTable: "attribute_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_attribute_definitions_code",
                table: "attribute_definitions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_attribute_options_definition_value",
                table: "attribute_options",
                columns: new[] { "attribute_definition_id", "value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_category_attributes_attribute_id",
                table: "category_attributes",
                column: "attribute_id");

            migrationBuilder.CreateIndex(
                name: "ix_category_attributes_category_id",
                table: "category_attributes",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ux_category_attributes_category_attribute",
                table: "category_attributes",
                columns: new[] { "category_id", "attribute_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_attribute_values_attribute_id",
                table: "product_attribute_values",
                column: "attribute_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_attribute_values_product_id",
                table: "product_attribute_values",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ux_product_attribute_values_product_attribute",
                table: "product_attribute_values",
                columns: new[] { "product_id", "attribute_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attribute_options");

            migrationBuilder.DropTable(
                name: "category_attributes");

            migrationBuilder.DropTable(
                name: "product_attribute_values");

            migrationBuilder.DropTable(
                name: "attribute_definitions");
        }
    }
}
