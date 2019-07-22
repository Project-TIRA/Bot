using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityModel.Migrations
{
    public partial class mh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MentalHealth_AgeRangeEnd",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MentalHealth_AgeRangeStart",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MentalHealth_Gender",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MentalHealth_HasWaitlist",
                table: "Organizations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MentalHealth_InPatientOpen",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MentalHealth_InPatientTotal",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MentalHealth_InPatientWaitlistLength",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MentalHealth_OutPatientOpen",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MentalHealth_OutPatientTotal",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MentalHealth_OutPatientWaitlistLength",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MentalHealth_AgeRangeEnd",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "MentalHealth_AgeRangeStart",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "MentalHealth_Gender",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "MentalHealth_HasWaitlist",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "MentalHealth_InPatientOpen",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "MentalHealth_InPatientTotal",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "MentalHealth_InPatientWaitlistLength",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "MentalHealth_OutPatientOpen",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "MentalHealth_OutPatientTotal",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "MentalHealth_OutPatientWaitlistLength",
                table: "Organizations");
        }
    }
}
