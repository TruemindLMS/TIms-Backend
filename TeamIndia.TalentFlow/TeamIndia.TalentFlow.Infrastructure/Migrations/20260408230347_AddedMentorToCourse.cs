using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamIndia.TalentFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedMentorToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MentorId",
                table: "Courses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_MentorId",
                table: "Courses",
                column: "MentorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_MentorId",
                table: "Courses",
                column: "MentorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_MentorId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_MentorId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "MentorId",
                table: "Courses");
        }
    }
}
