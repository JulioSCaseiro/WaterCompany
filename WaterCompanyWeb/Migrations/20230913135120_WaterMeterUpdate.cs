using Microsoft.EntityFrameworkCore.Migrations;

namespace WaterCompanyWeb.Migrations
{
    public partial class WaterMeterUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WaterMeter",
                table: "WaterMeter");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "WaterMeter");

            migrationBuilder.RenameTable(
                name: "WaterMeter",
                newName: "WaterMeters");

            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "WaterMeters",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "TotalConsumption",
                table: "WaterMeters",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "WaterMeters",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaterMeters",
                table: "WaterMeters",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WaterMeters_ClientId",
                table: "WaterMeters",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_WaterMeters_Clients_ClientId",
                table: "WaterMeters",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterMeters_Clients_ClientId",
                table: "WaterMeters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WaterMeters",
                table: "WaterMeters");

            migrationBuilder.DropIndex(
                name: "IX_WaterMeters_ClientId",
                table: "WaterMeters");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "WaterMeters");

            migrationBuilder.RenameTable(
                name: "WaterMeters",
                newName: "WaterMeter");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "WaterMeter",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalConsumption",
                table: "WaterMeter",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "WaterMeter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaterMeter",
                table: "WaterMeter",
                column: "Id");
        }
    }
}
