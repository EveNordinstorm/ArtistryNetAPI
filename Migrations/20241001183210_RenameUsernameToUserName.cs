using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtistryNetAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameUsernameToUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
            name: "UserName",
            table: "Users",
            type: "nvarchar(max)");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Posts",
                newName: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
