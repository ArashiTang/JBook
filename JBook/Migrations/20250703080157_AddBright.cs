using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JBook.Migrations
{
    /// <inheritdoc />
    public partial class AddBright : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Brightness",
                table: "DocumentSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brightness",
                table: "DocumentSettings");
        }
    }
}
