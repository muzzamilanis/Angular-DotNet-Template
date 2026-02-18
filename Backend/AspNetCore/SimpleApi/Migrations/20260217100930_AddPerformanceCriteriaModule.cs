using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceCriteriaModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerformanceCriterias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserRole = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    WeightPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    MeasurementMethod = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ScoringMethod = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TargetValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    MinimumThreshold = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    MaximumThreshold = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByAI = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceCriterias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerformanceCriterias_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPerformanceScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PerformanceCriteriaId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    AchievedValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    EvaluationPeriod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EvaluatorUserId = table.Column<int>(type: "integer", nullable: true),
                    Comments = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPerformanceScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPerformanceScores_PerformanceCriterias_PerformanceCrite~",
                        column: x => x.PerformanceCriteriaId,
                        principalTable: "PerformanceCriterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPerformanceScores_Users_EvaluatorUserId",
                        column: x => x.EvaluatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserPerformanceScores_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCriterias_CompanyId",
                table: "PerformanceCriterias",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceScores_EvaluatorUserId",
                table: "UserPerformanceScores",
                column: "EvaluatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceScores_PerformanceCriteriaId",
                table: "UserPerformanceScores",
                column: "PerformanceCriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceScores_UserId",
                table: "UserPerformanceScores",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPerformanceScores");

            migrationBuilder.DropTable(
                name: "PerformanceCriterias");
        }
    }
}
