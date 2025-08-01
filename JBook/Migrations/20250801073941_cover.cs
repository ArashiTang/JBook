using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JBook.Migrations
{
    /// <inheritdoc />
    public partial class cover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CoverImage",
                table: "Documents",
                newName: "CoverPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CoverPath",
                table: "Documents",
                newName: "CoverImage");
        }
    }
}
