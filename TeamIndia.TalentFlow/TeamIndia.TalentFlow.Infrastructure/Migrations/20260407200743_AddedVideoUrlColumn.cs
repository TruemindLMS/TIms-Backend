using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamIndia.TalentFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedVideoUrlColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Lessons",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Lessons");
        }
    }
}
