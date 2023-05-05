using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UphReport.Migrations
{
    /// <inheritdoc />
    public partial class i4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaveAPIKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    APIKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreditsRemaining = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaveAPIKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DomainName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCheck = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageSpeedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<float>(type: "real", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    Strategy = table.Column<int>(type: "int", nullable: false),
                    PSIVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WebPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSpeedReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSpeedReports_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PageSpeedReports_WebPages_WebPageId",
                        column: x => x.WebPageId,
                        principalTable: "WebPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageSpeedElements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ElementName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeOfResult = table.Column<int>(type: "int", nullable: false),
                    PageSpeedReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSpeedElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSpeedElements_PageSpeedReports_PageSpeedReportId",
                        column: x => x.PageSpeedReportId,
                        principalTable: "PageSpeedReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageSpeedSubElements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Snippet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Selector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PageSpeedElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSpeedSubElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSpeedSubElements_PageSpeedElements_PageSpeedElementId",
                        column: x => x.PageSpeedElementId,
                        principalTable: "PageSpeedElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageSpeedElements_PageSpeedReportId",
                table: "PageSpeedElements",
                column: "PageSpeedReportId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSpeedReports_CreatedById",
                table: "PageSpeedReports",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PageSpeedReports_WebPageId",
                table: "PageSpeedReports",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSpeedSubElements_PageSpeedElementId",
                table: "PageSpeedSubElements",
                column: "PageSpeedElementId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageSpeedSubElements");

            migrationBuilder.DropTable(
                name: "WaveAPIKeys");

            migrationBuilder.DropTable(
                name: "PageSpeedElements");

            migrationBuilder.DropTable(
                name: "PageSpeedReports");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WebPages");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
