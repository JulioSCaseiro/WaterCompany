using Microsoft.EntityFrameworkCore.Migrations;

namespace WaterCompanyWeb.Migrations
{
    public partial class CreateStaff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "Clients",
                newName: "ZIPCode");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Clients",
                newName: "PhoneNumber");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZIPCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NIF = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staff_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Staff_UserId",
                table: "Staff",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "ZIPCode",
                table: "Clients",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Clients",
                newName: "Phone");

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "Clients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
