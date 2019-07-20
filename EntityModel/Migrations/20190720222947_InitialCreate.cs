using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityModel.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpenBeds",
                table: "Snapshots",
                newName: "BedsWaitlist");

            migrationBuilder.RenameColumn(
                name: "TotalBeds",
                table: "Organizations",
                newName: "BedsTotal");

            migrationBuilder.AddColumn<int>(
                name: "BedsOpen",
                table: "Snapshots",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "BedsWaitlist",
                table: "Organizations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BedsOpen",
                table: "Snapshots");

            migrationBuilder.DropColumn(
                name: "BedsWaitlist",
                table: "Organizations");

            migrationBuilder.RenameColumn(
                name: "BedsWaitlist",
                table: "Snapshots",
                newName: "OpenBeds");

            migrationBuilder.RenameColumn(
                name: "BedsTotal",
                table: "Organizations",
                newName: "TotalBeds");
        }
    }
}
