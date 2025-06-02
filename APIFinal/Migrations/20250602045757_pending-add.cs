using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFinal.Migrations
{
    /// <inheritdoc />
    public partial class pendingadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PendingICS",
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
                    FundCluster = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ICSSDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IcsDateReceived = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingICS", x => x.ICSID);
                });

            migrationBuilder.CreateTable(
                name: "PendingPAR",
                columns: table => new
                {
                    ParID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ItemCode = table.Column<int>(type: "int", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DprHeadName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Classification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FundCls = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceCopies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    refNo = table.Column<int>(type: "int", nullable: false),
                    _value = table.Column<float>(type: "real", nullable: false),
                    ParQty = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingPAR", x => x.ParID);
                });

            migrationBuilder.CreateTable(
                name: "PendingSur",
                columns: table => new
                {
                    archiveID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrpName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    archiveDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false),
                    Clasification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Copies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemCond = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SurQTY = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingSur", x => x.archiveID);
                });

            migrationBuilder.CreateTable(
                name: "PendingTrans",
                columns: table => new
                {
                    PtrId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CstCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateTransferred = table.Column<DateOnly>(type: "date", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rvName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransferType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fundccl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    from = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    apprvdBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    designationOf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    approvedByDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    releaseBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    designationRelease = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    releaseByDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    receivedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    designationReceived = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    receivedByDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateAcquired = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingTrans", x => x.PtrId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingICS");

            migrationBuilder.DropTable(
                name: "PendingPAR");

            migrationBuilder.DropTable(
                name: "PendingSur");

            migrationBuilder.DropTable(
                name: "PendingTrans");
        }
    }
}
