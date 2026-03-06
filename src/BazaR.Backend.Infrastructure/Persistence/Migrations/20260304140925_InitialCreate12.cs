using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "users",
                type: "text",
                maxLength: 0,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 0,
                oldNullable: true,
                oldComment: "User's phone number");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "User's last name");

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "User's first name");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Image_CreatedAt",
                table: "categories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Image_Id",
                table: "categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_content_type",
                table: "categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "image_size_bytes",
                table: "categories",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_storage_key",
                table: "categories",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "categories",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_avatars",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    storage_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    content_type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_avatars", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_avatars_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_categories_image_storage_key",
                table: "categories",
                column: "image_storage_key");

            migrationBuilder.AddCheckConstraint(
                name: "ck_categories_image_pair",
                table: "categories",
                sql: "(image_url IS NULL AND image_storage_key IS NULL) OR (image_url IS NOT NULL AND image_storage_key IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_categories_image_size_positive_or_null",
                table: "categories",
                sql: "image_size_bytes IS NULL OR image_size_bytes > 0");

            migrationBuilder.CreateIndex(
                name: "ux_user_avatars_user_id",
                table: "user_avatars",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_avatars");

            migrationBuilder.DropIndex(
                name: "ix_categories_image_storage_key",
                table: "categories");

            migrationBuilder.DropCheckConstraint(
                name: "ck_categories_image_pair",
                table: "categories");

            migrationBuilder.DropCheckConstraint(
                name: "ck_categories_image_size_positive_or_null",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "Image_CreatedAt",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "Image_Id",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "image_content_type",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "image_size_bytes",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "image_storage_key",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "categories");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "users",
                type: "text",
                maxLength: 0,
                nullable: true,
                comment: "User's phone number",
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 0,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "User's last name",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "User's first name",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
