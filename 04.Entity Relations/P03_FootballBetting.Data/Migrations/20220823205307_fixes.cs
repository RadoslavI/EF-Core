using Microsoft.EntityFrameworkCore.Migrations;

namespace P03_FootballBetting.Data.Migrations
{
    public partial class fixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SquatNumber",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "SquadNumber",
                table: "Players",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SquadNumber",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "SquatNumber",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
