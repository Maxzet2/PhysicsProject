using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhysicsProject.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SectionProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestTimeLimitSeconds",
                table: "Sections",
                type: "integer",
                nullable: false,
                defaultValue: 600);

            migrationBuilder.CreateTable(
                name: "SectionProgress",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptCycle = table.Column<int>(type: "integer", nullable: false),
                    AttemptsUsedInCycle = table.Column<int>(type: "integer", nullable: false),
                    ActiveTestSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActiveTestExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastTrainingCompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastTestPassedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionProgress", x => new { x.UserId, x.SectionId });
                    table.ForeignKey(
                        name: "FK_SectionProgress_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionTestAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptIndex = table.Column<int>(type: "integer", nullable: false),
                    AttemptCycle = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TimeLimitSeconds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionTestAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionTestAttempts_SectionProgress_UserId_SectionId",
                        columns: x => new { x.UserId, x.SectionId },
                        principalTable: "SectionProgress",
                        principalColumns: new[] { "UserId", "SectionId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SectionProgress_SectionId",
                table: "SectionProgress",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionTestAttempts_SessionId",
                table: "SectionTestAttempts",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SectionTestAttempts_UserId_SectionId",
                table: "SectionTestAttempts",
                columns: new[] { "UserId", "SectionId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SectionTestAttempts");

            migrationBuilder.DropTable(
                name: "SectionProgress");

            migrationBuilder.DropColumn(
                name: "TestTimeLimitSeconds",
                table: "Sections");
        }
    }
}
