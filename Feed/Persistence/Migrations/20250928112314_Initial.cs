using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feed.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Interval = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Artifact",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FeedId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fields_Item_Selector = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fields_Item_Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Fields_Title_Selector = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fields_Title_Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Fields_Link_Selector = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fields_Link_Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Fields_Description_Selector = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fields_Description_Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Fields_Date_Selector = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fields_Date_Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artifact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Artifact_Plans_FeedId",
                        column: x => x.FeedId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artifact_FeedId",
                table: "Artifact",
                column: "FeedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Artifact");

            migrationBuilder.DropTable(
                name: "Plans");
        }
    }
}
