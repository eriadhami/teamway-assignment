using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkPlan.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Plans_Shifts_ShiftID",
                        column: x => x.ShiftID,
                        principalTable: "Shifts",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Plans_Workers_WorkerID",
                        column: x => x.WorkerID,
                        principalTable: "Workers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plans_ShiftID",
                table: "Plans",
                column: "ShiftID");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_WorkerID",
                table: "Plans",
                column: "WorkerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plans");
        }
    }
}
