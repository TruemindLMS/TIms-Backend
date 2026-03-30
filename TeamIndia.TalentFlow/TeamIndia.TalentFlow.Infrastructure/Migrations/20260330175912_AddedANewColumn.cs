using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamIndia.TalentFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedANewColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMentorApproved",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMentorApproved",
                table: "AspNetUsers");
        }
    }
}
