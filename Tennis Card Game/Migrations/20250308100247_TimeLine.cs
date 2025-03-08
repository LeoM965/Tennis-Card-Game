using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tennis_Card_Game.Migrations
{
    /// <inheritdoc />
    public partial class TimeLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MatchOrder",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Round",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchOrder",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Round",
                table: "Matches");
        }
    }
}
