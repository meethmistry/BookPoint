using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookPoint.Migrations
{
    /// <inheritdoc />
    public partial class AddSamplePdfToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SamplePdfPath",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SamplePdfPath",
                table: "Books");
        }
    }
}
