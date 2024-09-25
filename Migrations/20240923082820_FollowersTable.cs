using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtistryNetAPI.Migrations
{
    /// <inheritdoc />
    public partial class FollowersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Followers",
                columns: table => new
                {
                    FollowerID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FollowID = table.Column<int>(type: "int", nullable: false),
                    FollowedID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followers", x => x.FollowerID);
                    table.ForeignKey(
                        name: "FK_Followers_Users_FollowedID",
                        column: x => x.FollowedID,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Followers_Users_FollowerID",
                        column: x => x.FollowerID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowedID",
                table: "Followers",
                column: "FollowedID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Followers");
        }
    }
}
