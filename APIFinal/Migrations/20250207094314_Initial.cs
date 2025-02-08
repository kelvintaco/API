using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFinal.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchievedTransactions",
                columns: table => new
                {
                    archiveID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrpName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DprName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    archiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    ItemCond = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SurQTY = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchievedTransactions", x => x.archiveID);
                });

            migrationBuilder.CreateTable(
                name: "Custodians",
                columns: table => new
                {
                    CSTCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CSTName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DPRName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Custodians", x => x.CSTCode);
                });

            migrationBuilder.CreateTable(
                name: "ICS",
                columns: table => new
                {
                    ICSID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CSTCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ICSName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ICSPrice = table.Column<double>(type: "float", nullable: false),
                    Life = table.Column<int>(type: "int", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    IcsDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ICS", x => x.ICSID);
                });

            migrationBuilder.CreateTable(
                name: "ItemDisposal",
                columns: table => new
                {
                    NonServCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    disposalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    disposalDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDisposal", x => x.NonServCode);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemCode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    isonBorrow = table.Column<int>(type: "int", nullable: false),
                    isnotonBorrow = table.Column<int>(type: "int", nullable: false),
                    Servicable = table.Column<bool>(type: "bit", nullable: false),
                    NonServ = table.Column<bool>(type: "bit", nullable: false),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemCode);
                });

            migrationBuilder.CreateTable(
                name: "PAR",
                columns: table => new
                {
                    ParID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ItemCode = table.Column<int>(type: "int", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DprName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParDate = table.Column<DateOnly>(type: "date", nullable: false),
                    refNo = table.Column<int>(type: "int", nullable: false),
                    ParQty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAR", x => x.ParID);
                });

            migrationBuilder.CreateTable(
                name: "Transfer",
                columns: table => new
                {
                    PtrId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CstCode = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateTransferred = table.Column<DateOnly>(type: "date", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    receiveName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransferType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfer", x => x.PtrId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchievedTransactions");

            migrationBuilder.DropTable(
                name: "Custodians");

            migrationBuilder.DropTable(
                name: "ICS");

            migrationBuilder.DropTable(
                name: "ItemDisposal");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "PAR");

            migrationBuilder.DropTable(
                name: "Transfer");
        }
    }
}
