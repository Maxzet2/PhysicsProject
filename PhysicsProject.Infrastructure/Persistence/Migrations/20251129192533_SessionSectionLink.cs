using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhysicsProject.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SessionSectionLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SectionId",
                table: "TestSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestSessions_SectionId",
                table: "TestSessions",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestSessions_Sections_SectionId",
                table: "TestSessions",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestSessions_Sections_SectionId",
                table: "TestSessions");

            migrationBuilder.DropIndex(
                name: "IX_TestSessions_SectionId",
                table: "TestSessions");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "TestSessions");
        }
    }
}
