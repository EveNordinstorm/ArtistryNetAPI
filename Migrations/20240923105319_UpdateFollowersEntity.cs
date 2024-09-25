using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtistryNetAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFollowersEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Followers",
                columns: table => new
                {
                    FollowID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FollowerID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FollowedID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followers", x => x.FollowID);
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

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowerID",
                table: "Followers",
                column: "FollowerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Followers");
        }
    }
}
