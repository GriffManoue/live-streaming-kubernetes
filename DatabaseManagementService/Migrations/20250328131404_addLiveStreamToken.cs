using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class addLiveStreamToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StreamKey",
                table: "LiveStreams",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamKey",
                table: "LiveStreams");
        }
    }
}
