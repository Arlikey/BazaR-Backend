using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""CREATE EXTENSION IF NOT EXISTS "pgcrypto";""");

            migrationBuilder.DropPrimaryKey(
                name: "PK_product_attribute_value_option_ids",
                table: "product_attribute_value_option_ids");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "product_attribute_value_option_ids",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE product_attribute_value_option_ids
                SET id = gen_random_uuid()
                WHERE id IS NULL;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "product_attribute_value_option_ids",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_product_attribute_value_option_ids",
                table: "product_attribute_value_option_ids",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_pav_option_ids_pav_id",
                table: "product_attribute_value_option_ids",
                column: "product_attribute_value_id");

            migrationBuilder.CreateIndex(
                name: "ux_pav_option_ids_pav_option",
                table: "product_attribute_value_option_ids",
                columns: new[] { "product_attribute_value_id", "option_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_product_attribute_value_option_ids",
                table: "product_attribute_value_option_ids");

            migrationBuilder.DropIndex(
                name: "ix_pav_option_ids_pav_id",
                table: "product_attribute_value_option_ids");

            migrationBuilder.DropIndex(
                name: "ux_pav_option_ids_pav_option",
                table: "product_attribute_value_option_ids");

            migrationBuilder.DropColumn(
                name: "id",
                table: "product_attribute_value_option_ids");

            migrationBuilder.AddPrimaryKey(
                name: "PK_product_attribute_value_option_ids",
                table: "product_attribute_value_option_ids",
                columns: new[] { "product_attribute_value_id", "option_id" });
        }
    }
}