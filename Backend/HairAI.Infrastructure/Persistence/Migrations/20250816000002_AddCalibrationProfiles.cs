using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HairAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCalibrationProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalibrationProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CalibrationData = table.Column<string>(type: "jsonb", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalibrationProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalibrationProfiles_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationProfiles_ClinicId",
                table: "CalibrationProfiles",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationProfiles_ClinicId_ProfileName_Version",
                table: "CalibrationProfiles",
                columns: new[] { "ClinicId", "ProfileName", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalibrationProfiles");
        }
    }
}