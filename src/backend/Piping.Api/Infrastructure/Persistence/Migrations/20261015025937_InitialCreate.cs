using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piping.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PipeOdLookups",
                columns: table => new
                {
                    PipeSizeInch = table.Column<decimal>(type: "TEXT", nullable: false),
                    ActualOdMm = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipeOdLookups", x => x.PipeSizeInch);
                });

            migrationBuilder.CreateTable(
                name: "PipingLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LineNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FromDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ToDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DrawingNumber = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Service = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Material = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    InserviceDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    PipeSizeInch = table.Column<decimal>(type: "TEXT", nullable: true),
                    OriginalThicknessMm = table.Column<decimal>(type: "TEXT", nullable: true),
                    Stress = table.Column<decimal>(type: "TEXT", nullable: true),
                    JointEfficiency = table.Column<decimal>(type: "TEXT", nullable: true),
                    CorrosionAllowanceMm = table.Column<decimal>(type: "TEXT", nullable: true),
                    DesignLife = table.Column<decimal>(type: "TEXT", nullable: true),
                    DesignPressure = table.Column<decimal>(type: "TEXT", nullable: true),
                    OperatingPressure = table.Column<decimal>(type: "TEXT", nullable: true),
                    DesignTemperature = table.Column<decimal>(type: "TEXT", nullable: true),
                    OperatingTemperature = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipingLines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StructuralThicknessRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MinPipeSizeInch = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaxPipeSizeInch = table.Column<decimal>(type: "TEXT", nullable: true),
                    StructuralThicknessMm = table.Column<decimal>(type: "TEXT", nullable: false),
                    RuleLabel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructuralThicknessRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cmls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PipingLineId = table.Column<int>(type: "INTEGER", nullable: false),
                    CmlNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cmls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cmls_PipingLines_PipingLineId",
                        column: x => x.PipingLineId,
                        principalTable: "PipingLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CmlId = table.Column<int>(type: "INTEGER", nullable: false),
                    TpNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    TpDescription = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestPoints_Cmls_CmlId",
                        column: x => x.CmlId,
                        principalTable: "Cmls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThicknessReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestPointId = table.Column<int>(type: "INTEGER", nullable: false),
                    InspectionDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ActualThicknessMm = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThicknessReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThicknessReadings_TestPoints_TestPointId",
                        column: x => x.TestPointId,
                        principalTable: "TestPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cmls_PipingLineId_CmlNumber",
                table: "Cmls",
                columns: new[] { "PipingLineId", "CmlNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PipingLines_LineNumber",
                table: "PipingLines",
                column: "LineNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestPoints_CmlId_TpNumber",
                table: "TestPoints",
                columns: new[] { "CmlId", "TpNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThicknessReadings_TestPointId_InspectionDate",
                table: "ThicknessReadings",
                columns: new[] { "TestPointId", "InspectionDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PipeOdLookups");

            migrationBuilder.DropTable(
                name: "StructuralThicknessRules");

            migrationBuilder.DropTable(
                name: "ThicknessReadings");

            migrationBuilder.DropTable(
                name: "TestPoints");

            migrationBuilder.DropTable(
                name: "Cmls");

            migrationBuilder.DropTable(
                name: "PipingLines");
        }
    }
}
