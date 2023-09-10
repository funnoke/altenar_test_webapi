using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace altenar_test_webapi.Migrations
{
    /// <inheritdoc />
    public partial class NameTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateDataBet",
                table: "Bets",
                newName: "CreateDateBet");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateDateBet",
                table: "Bets",
                newName: "CreateDataBet");
        }
    }
}
