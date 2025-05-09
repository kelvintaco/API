using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFinal.Migrations
{
    /// <inheritdoc />
    public partial class transferdesignationfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "designation",
                table: "Transfer",
                newName: "designationOf");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "designationOf",
                table: "Transfer",
                newName: "designation");
        }
    }
}
