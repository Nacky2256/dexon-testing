using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piping.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StructuralRuleLabelsUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StructuralThicknessRules_RuleLabel",
                table: "StructuralThicknessRules",
                column: "RuleLabel",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StructuralThicknessRules_RuleLabel",
                table: "StructuralThicknessRules");
        }
    }
}
