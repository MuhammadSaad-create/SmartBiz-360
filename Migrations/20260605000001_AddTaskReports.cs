using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartBiz360.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalTasks = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedTasks = table.Column<int>(type: "INTEGER", nullable: false),
                    InProgressTasks = table.Column<int>(type: "INTEGER", nullable: false),
                    OverdueTasks = table.Column<int>(type: "INTEGER", nullable: false),
                    ToDoTasks = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletionRate = table.Column<double>(type: "REAL", nullable: false),
                    OnTimeRate = table.Column<double>(type: "REAL", nullable: false),
                    Grade = table.Column<string>(type: "TEXT", nullable: false),
                    BonusEligible = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailSentTo = table.Column<string>(type: "TEXT", nullable: false),
                    EmailSent = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskReports_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskReports_EmployeeId",
                table: "TaskReports",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskReports_UserId",
                table: "TaskReports",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TaskReports");
        }
    }
}
