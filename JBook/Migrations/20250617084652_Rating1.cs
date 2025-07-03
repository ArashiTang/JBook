using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JBook.Migrations
{
    /// <inheritdoc />
    public partial class Rating1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Documents",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "Author", "FilePath" },
                values: new object[] { null, "uploads/sun_zi_art_of_war.txt" });

            migrationBuilder.UpdateData(
                table: "Documents",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "Author", "FilePath" },
                values: new object[] { null, "uploads/alive.txt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Documents");

            migrationBuilder.UpdateData(
                table: "Documents",
                keyColumn: "id",
                keyValue: 1,
                column: "FilePath",
                value: null);

            migrationBuilder.UpdateData(
                table: "Documents",
                keyColumn: "id",
                keyValue: 2,
                column: "FilePath",
                value: null);
        }
    }
}
