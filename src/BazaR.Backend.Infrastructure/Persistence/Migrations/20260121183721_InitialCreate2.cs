using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_categories_parent_category",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "ix_categories_name",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "version",
                table: "categories");

            migrationBuilder.RenameIndex(
                name: "ix_categories_parent_category_id",
                table: "categories",
                newName: "ix_categories_parent_id");

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_categories_parent_sort",
                table: "categories",
                columns: new[] { "parent_category_id", "sort_order" });

            migrationBuilder.AddCheckConstraint(
                name: "ck_categories_name_not_empty",
                table: "categories",
                sql: "char_length(name) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_categories_sort_order_non_negative",
                table: "categories",
                sql: "sort_order >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_categories_parent_sort",
                table: "categories");

            migrationBuilder.DropCheckConstraint(
                name: "ck_categories_name_not_empty",
                table: "categories");

            migrationBuilder.DropCheckConstraint(
                name: "ck_categories_sort_order_non_negative",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "categories");

            migrationBuilder.RenameIndex(
                name: "ix_categories_parent_id",
                table: "categories",
                newName: "ix_categories_parent_category_id");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "categories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "version",
                table: "categories",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "ix_categories_name",
                table: "categories",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_categories_parent_category",
                table: "categories",
                column: "parent_category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
