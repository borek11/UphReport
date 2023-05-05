using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UphReport.Migrations
{
    /// <inheritdoc />
    public partial class i2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaveReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    Strategy = table.Column<int>(type: "int", nullable: false),
                    WaveVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaveReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaveReports_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WaveReports_WebPages_WebPageId",
                        column: x => x.WebPageId,
                        principalTable: "WebPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaveElements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ElementName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeOfResult = table.Column<int>(type: "int", nullable: false),
                    WaveReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaveElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaveElements_WaveReports_WaveReportId",
                        column: x => x.WaveReportId,
                        principalTable: "WaveReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaveSubElements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Selector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WaveElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaveSubElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaveSubElements_WaveElements_WaveElementId",
                        column: x => x.WaveElementId,
                        principalTable: "WaveElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaveElements_WaveReportId",
                table: "WaveElements",
                column: "WaveReportId");

            migrationBuilder.CreateIndex(
                name: "IX_WaveReports_CreatedById",
                table: "WaveReports",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WaveReports_WebPageId",
                table: "WaveReports",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_WaveSubElements_WaveElementId",
                table: "WaveSubElements",
                column: "WaveElementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaveSubElements");

            migrationBuilder.DropTable(
                name: "WaveElements");

            migrationBuilder.DropTable(
                name: "WaveReports");
        }
    }
}
