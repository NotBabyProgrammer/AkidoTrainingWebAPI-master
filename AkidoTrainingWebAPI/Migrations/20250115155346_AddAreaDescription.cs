using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AkidoTrainingWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAreaDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Areas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Areas");
        }
    }
}
