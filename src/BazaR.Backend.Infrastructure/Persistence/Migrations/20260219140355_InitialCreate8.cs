using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "User's first name");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_login_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "User's last name");

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "users",
                type: "text",
                maxLength: 0,
                nullable: true,
                comment: "User's phone number");

            migrationBuilder.AlterColumn<string>(
                name: "suspension_reason",
                table: "sellers",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "last_rejection_reason",
                table: "sellers",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "close_reason",
                table: "sellers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "closed_at",
                table: "sellers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "closed_by",
                table: "sellers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "sellers",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "logo_url",
                table: "sellers",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "submitted_at",
                table: "sellers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "suspended_at",
                table: "sellers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "suspended_by",
                table: "sellers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "sellers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "ix_users_full_name",
                table: "users",
                columns: new[] { "first_name", "last_name" });

            migrationBuilder.CreateIndex(
                name: "ix_users_last_login_at",
                table: "users",
                column: "last_login_at");

            migrationBuilder.CreateIndex(
                name: "ix_users_last_name",
                table: "users",
                column: "last_name");

            migrationBuilder.CreateIndex(
                name: "ix_users_phone",
                table: "users",
                column: "phone",
                unique: true,
                filter: "phone IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_sellers_country_code",
                table: "sellers",
                column: "country_code");

            migrationBuilder.CreateIndex(
                name: "ix_sellers_submitted_at",
                table: "sellers",
                column: "submitted_at");

            migrationBuilder.AddCheckConstraint(
                name: "ck_sellers_country_code_len",
                table: "sellers",
                sql: "char_length(country_code) = 2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_full_name",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_last_login_at",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_last_name",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_phone",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_sellers_country_code",
                table: "sellers");

            migrationBuilder.DropIndex(
                name: "ix_sellers_submitted_at",
                table: "sellers");

            migrationBuilder.DropCheckConstraint(
                name: "ck_sellers_country_code_len",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "first_name",
                table: "users");

            migrationBuilder.DropColumn(
                name: "last_login_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "last_name",
                table: "users");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "users");

            migrationBuilder.DropColumn(
                name: "close_reason",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "closed_at",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "closed_by",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "description",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "logo_url",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "submitted_at",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "suspended_at",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "suspended_by",
                table: "sellers");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "sellers");

            migrationBuilder.AlterColumn<string>(
                name: "suspension_reason",
                table: "sellers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "last_rejection_reason",
                table: "sellers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
