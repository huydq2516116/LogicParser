using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicParser.Api.Migrations
{
    /// <inheritdoc />
    public partial class DeleteUnneededRows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClauseCount",
                table: "Saves");

            migrationBuilder.DropColumn(
                name: "ResultCount",
                table: "Saves");

            migrationBuilder.DropColumn(
                name: "VariableCount",
                table: "Saves");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClauseCount",
                table: "Saves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResultCount",
                table: "Saves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VariableCount",
                table: "Saves",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
