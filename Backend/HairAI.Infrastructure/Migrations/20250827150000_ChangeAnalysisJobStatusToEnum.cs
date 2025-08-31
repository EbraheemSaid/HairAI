using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HairAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAnalysisJobStatusToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Change the Status column from string to integer (for enum)
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "AnalysisJobs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert the Status column from integer back to string
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AnalysisJobs",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}