using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFinal.Migrations
{
    /// <inheritdoc />
    public partial class transferdatefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "dateAcquired",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dateAcquired",
                table: "Transfer");
        }
    }
}
