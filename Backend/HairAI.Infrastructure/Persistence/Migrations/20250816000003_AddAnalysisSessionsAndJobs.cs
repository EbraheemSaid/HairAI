using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HairAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisSessionsAndJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    SessionDate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "now()"),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FinalReportData = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisSessions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CalibrationProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    LocationTag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageStorageKey = table.Column<string>(type: "text", nullable: false),
                    AnnotatedImageKey = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AnalysisResult = table.Column<string>(type: "jsonb", nullable: true),
                    DoctorNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    ProcessingTimeMs = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisJobs_AnalysisSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "AnalysisSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalysisJobs_CalibrationProfiles_CalibrationProfileId",
                        column: x => x.CalibrationProfileId,
                        principalTable: "CalibrationProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalysisJobs_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisJobs_CalibrationProfileId",
                table: "AnalysisJobs",
                column: "CalibrationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisJobs_PatientId",
                table: "AnalysisJobs",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisJobs_SessionId",
                table: "AnalysisJobs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisJobs_Status",
                table: "AnalysisJobs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisSessions_PatientId",
                table: "AnalysisSessions",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisJobs");

            migrationBuilder.DropTable(
                name: "AnalysisSessions");
        }
    }
}