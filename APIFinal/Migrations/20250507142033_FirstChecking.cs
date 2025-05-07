using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFinal.Migrations
{
    /// <inheritdoc />
    public partial class FirstChecking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DprName",
                table: "PAR",
                newName: "SourceCopies");

            migrationBuilder.RenameColumn(
                name: "DprName",
                table: "ArchievedTransactions",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "approvedBy",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "approvedByDate",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "designation",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "designationReceived",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "designationRelease",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "from",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "fundcluster",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reason",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "receivedBy",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "receivedByDate",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "releaseBy",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "releaseByDate",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "to",
                table: "Transfer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Classification",
                table: "PAR",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DprHeadName",
                table: "PAR",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FundCls",
                table: "PAR",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FundCluster",
                table: "ICS",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "IcsDateReceived",
                table: "ICS",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "ICS",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Clasification",
                table: "ArchievedTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Copies",
                table: "ArchievedTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "approvedBy",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "approvedByDate",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "designation",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "designationReceived",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "designationRelease",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "from",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "fundcluster",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "reason",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "receivedBy",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "receivedByDate",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "releaseBy",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "releaseByDate",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "to",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "Classification",
                table: "PAR");

            migrationBuilder.DropColumn(
                name: "DprHeadName",
                table: "PAR");

            migrationBuilder.DropColumn(
                name: "FundCls",
                table: "PAR");

            migrationBuilder.DropColumn(
                name: "FundCluster",
                table: "ICS");

            migrationBuilder.DropColumn(
                name: "IcsDateReceived",
                table: "ICS");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "ICS");

            migrationBuilder.DropColumn(
                name: "Clasification",
                table: "ArchievedTransactions");

            migrationBuilder.DropColumn(
                name: "Copies",
                table: "ArchievedTransactions");

            migrationBuilder.RenameColumn(
                name: "SourceCopies",
                table: "PAR",
                newName: "DprName");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ArchievedTransactions",
                newName: "DprName");
        }
    }
}
