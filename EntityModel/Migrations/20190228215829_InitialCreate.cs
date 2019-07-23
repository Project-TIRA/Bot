using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityModel.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    IsComplete = table.Column<bool>(nullable: false),
                    IsVerified = table.Column<bool>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Zip = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    AgeRangeStart = table.Column<int>(nullable: false),
                    AgeRangeEnd = table.Column<int>(nullable: false),
                    UpdateFrequency = table.Column<int>(nullable: false),
                    TotalBeds = table.Column<int>(nullable: false),
                    HasJobTrainingServices = table.Column<bool>(nullable:false),
                    HasJobTrainingWaitlist = table.Column<bool>(nullable:false),
                    OpenJobTrainingPositions = table.Column<int>(nullable:false),
                    TotalJobTrainingPositions = table.Column<int>(nullable:false),
                    JobTrainingWaitlistPositions = table.Column<int>(nullable:false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    OpenBeds = table.Column<int>(nullable: false),
                    OpenJobTrainingPositions = table.Column<int>(nullable: false),
                    JobTrainingWaitlistPositions = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Snapshots_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_PhoneNumber",
                table: "Organizations",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Snapshots_OrganizationId",
                table: "Snapshots",
                column: "OrganizationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Snapshots");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
