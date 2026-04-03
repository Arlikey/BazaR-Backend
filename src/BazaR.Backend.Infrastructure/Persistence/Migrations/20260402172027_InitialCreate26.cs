using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaR.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product_rating_summaries",
                columns: table => new
                {
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    reviews_count = table.Column<int>(type: "integer", nullable: false),
                    five_stars_count = table.Column<int>(type: "integer", nullable: false),
                    four_stars_count = table.Column<int>(type: "integer", nullable: false),
                    three_stars_count = table.Column<int>(type: "integer", nullable: false),
                    two_stars_count = table.Column<int>(type: "integer", nullable: false),
                    one_star_count = table.Column<int>(type: "integer", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_rating_summaries", x => x.product_id);
                    table.CheckConstraint("ck_product_rating_summaries_average_rating_range", "average_rating >= 0 AND average_rating <= 5");
                    table.CheckConstraint("ck_product_rating_summaries_five_stars_count_non_negative", "five_stars_count >= 0");
                    table.CheckConstraint("ck_product_rating_summaries_four_stars_count_non_negative", "four_stars_count >= 0");
                    table.CheckConstraint("ck_product_rating_summaries_one_star_count_non_negative", "one_star_count >= 0");
                    table.CheckConstraint("ck_product_rating_summaries_reviews_count_non_negative", "reviews_count >= 0");
                    table.CheckConstraint("ck_product_rating_summaries_three_stars_count_non_negative", "three_stars_count >= 0");
                    table.CheckConstraint("ck_product_rating_summaries_two_stars_count_non_negative", "two_stars_count >= 0");
                });

            migrationBuilder.CreateTable(
                name: "product_reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    moderated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_reviews", x => x.id);
                    table.CheckConstraint("ck_product_reviews_rating_range", "rating >= 1 AND rating <= 5");
                });

            migrationBuilder.CreateTable(
                name: "seller_rating_summaries",
                columns: table => new
                {
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    reviews_count = table.Column<int>(type: "integer", nullable: false),
                    five_stars_count = table.Column<int>(type: "integer", nullable: false),
                    four_stars_count = table.Column<int>(type: "integer", nullable: false),
                    three_stars_count = table.Column<int>(type: "integer", nullable: false),
                    two_stars_count = table.Column<int>(type: "integer", nullable: false),
                    one_star_count = table.Column<int>(type: "integer", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seller_rating_summaries", x => x.seller_id);
                    table.CheckConstraint("ck_seller_rating_summaries_average_rating_range", "average_rating >= 0 AND average_rating <= 5");
                    table.CheckConstraint("ck_seller_rating_summaries_five_stars_count_non_negative", "five_stars_count >= 0");
                    table.CheckConstraint("ck_seller_rating_summaries_four_stars_count_non_negative", "four_stars_count >= 0");
                    table.CheckConstraint("ck_seller_rating_summaries_one_star_count_non_negative", "one_star_count >= 0");
                    table.CheckConstraint("ck_seller_rating_summaries_reviews_count_non_negative", "reviews_count >= 0");
                    table.CheckConstraint("ck_seller_rating_summaries_three_stars_count_non_negative", "three_stars_count >= 0");
                    table.CheckConstraint("ck_seller_rating_summaries_two_stars_count_non_negative", "two_stars_count >= 0");
                });

            migrationBuilder.CreateTable(
                name: "seller_reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    moderated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seller_reviews", x => x.id);
                    table.CheckConstraint("ck_seller_reviews_rating_range", "rating >= 1 AND rating <= 5");
                });

            migrationBuilder.CreateTable(
                name: "product_review_votes",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_review_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_helpful = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_review_votes", x => new { x.product_review_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_product_review_votes_product_reviews_product_review_id",
                        column: x => x.product_review_id,
                        principalTable: "product_reviews",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seller_review_votes",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_review_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_helpful = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seller_review_votes", x => new { x.seller_review_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_seller_review_votes_seller_reviews_seller_review_id",
                        column: x => x.seller_review_id,
                        principalTable: "seller_reviews",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_rating_summaries_average_rating",
                table: "product_rating_summaries",
                column: "average_rating");

            migrationBuilder.CreateIndex(
                name: "ix_product_rating_summaries_reviews_count",
                table: "product_rating_summaries",
                column: "reviews_count");

            migrationBuilder.CreateIndex(
                name: "ix_product_review_votes_user_id",
                table: "product_review_votes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_reviews_author_user_id",
                table: "product_reviews",
                column: "author_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_reviews_created_at_utc",
                table: "product_reviews",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_product_reviews_product_id",
                table: "product_reviews",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_reviews_status",
                table: "product_reviews",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_product_reviews_product_author",
                table: "product_reviews",
                columns: new[] { "product_id", "author_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_seller_rating_summaries_average_rating",
                table: "seller_rating_summaries",
                column: "average_rating");

            migrationBuilder.CreateIndex(
                name: "ix_seller_rating_summaries_reviews_count",
                table: "seller_rating_summaries",
                column: "reviews_count");

            migrationBuilder.CreateIndex(
                name: "ix_seller_review_votes_user_id",
                table: "seller_review_votes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_seller_reviews_author_user_id",
                table: "seller_reviews",
                column: "author_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_seller_reviews_created_at_utc",
                table: "seller_reviews",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_seller_reviews_seller_id",
                table: "seller_reviews",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "ix_seller_reviews_status",
                table: "seller_reviews",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_seller_reviews_seller_author",
                table: "seller_reviews",
                columns: new[] { "seller_id", "author_user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_rating_summaries");

            migrationBuilder.DropTable(
                name: "product_review_votes");

            migrationBuilder.DropTable(
                name: "seller_rating_summaries");

            migrationBuilder.DropTable(
                name: "seller_review_votes");

            migrationBuilder.DropTable(
                name: "product_reviews");

            migrationBuilder.DropTable(
                name: "seller_reviews");
        }
    }
}
