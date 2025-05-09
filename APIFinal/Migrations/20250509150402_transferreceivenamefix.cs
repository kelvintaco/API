using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFinal.Migrations
{
    /// <inheritdoc />
    public partial class transferreceivenamefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "receiveName",
                table: "Transfer",
                newName: "rvName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "rvName",
                table: "Transfer",
                newName: "receiveName");
        }
    }
}
