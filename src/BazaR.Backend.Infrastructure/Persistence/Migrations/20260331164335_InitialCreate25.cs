using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "filter_presentation_type",
                table: "category_attributes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_visible_in_specifications",
                table: "category_attributes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_visible_on_product_card",
                table: "category_attributes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "product_attribute_value_option_ids",
                columns: table => new
                {
                    option_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_attribute_value_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_attribute_value_option_ids", x => new { x.product_attribute_value_id, x.option_id });
                    table.ForeignKey(
                        name: "FK_product_attribute_value_option_ids_product_attribute_values~",
                        column: x => x.product_attribute_value_id,
                        principalTable: "product_attribute_values",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_category_attributes_is_filterable",
                table: "category_attributes",
                column: "is_filterable");

            migrationBuilder.CreateIndex(
                name: "ix_category_attributes_section_order",
                table: "category_attributes",
                column: "section_order");

            migrationBuilder.CreateIndex(
                name: "ix_category_attributes_sort_order",
                table: "category_attributes",
                column: "sort_order");

            migrationBuilder.AddCheckConstraint(
                name: "ck_category_attributes_filter_presentation_type_requires_filte~",
                table: "category_attributes",
                sql: "filter_presentation_type IS NULL OR is_filterable = TRUE");

            migrationBuilder.CreateIndex(
                name: "ix_pav_option_ids_option_id",
                table: "product_attribute_value_option_ids",
                column: "option_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_attribute_value_option_ids");

            migrationBuilder.DropIndex(
                name: "ix_category_attributes_is_filterable",
                table: "category_attributes");

            migrationBuilder.DropIndex(
                name: "ix_category_attributes_section_order",
                table: "category_attributes");

            migrationBuilder.DropIndex(
                name: "ix_category_attributes_sort_order",
                table: "category_attributes");

            migrationBuilder.DropCheckConstraint(
                name: "ck_category_attributes_filter_presentation_type_requires_filte~",
                table: "category_attributes");

            migrationBuilder.DropColumn(
                name: "filter_presentation_type",
                table: "category_attributes");

            migrationBuilder.DropColumn(
                name: "is_visible_in_specifications",
                table: "category_attributes");

            migrationBuilder.DropColumn(
                name: "is_visible_on_product_card",
                table: "category_attributes");
        }
    }
}
