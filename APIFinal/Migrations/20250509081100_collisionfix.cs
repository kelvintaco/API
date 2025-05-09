using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFinal.Migrations
{
    /// <inheritdoc />
    public partial class collisionfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ICSDate",
                table: "ICS",
                newName: "ICSSDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ICSSDate",
                table: "ICS",
                newName: "ICSDate");
        }
    }
}
