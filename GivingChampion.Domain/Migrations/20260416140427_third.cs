using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DonationOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TargetLocation = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Receipt = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImpactReport = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DonationRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonationOrders_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrgName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrgType = table.Column<int>(type: "integer", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    ProjectsCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VolunteerOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ScheduleDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ServiceRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    VolunteerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Volunteers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Skills = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Availability = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TotalHours = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volunteers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Volunteers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DonateAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountRemaining = table.Column<decimal>(type: "numeric", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    UrgencyLevel = table.Column<int>(type: "integer", nullable: false),
                    BriefDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PartnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonationRequests_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RequiredSkill = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    UrgencyLevel = table.Column<int>(type: "integer", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BriefDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PartnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IssuedTo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VolunteerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalHours = table.Column<int>(type: "integer", nullable: false),
                    QrCode = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificates_Volunteers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "Volunteers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_VolunteerId",
                table: "Certificates",
                column: "VolunteerId");

            migrationBuilder.CreateIndex(
                name: "IX_DonationOrders_DonorId",
                table: "DonationOrders",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_DonationRequests_PartnerId",
                table: "DonationRequests",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_PartnerId",
                table: "ServiceRequests",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_UserId",
                table: "Volunteers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "DonationOrders");

            migrationBuilder.DropTable(
                name: "DonationRequests");

            migrationBuilder.DropTable(
                name: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "VolunteerOrders");

            migrationBuilder.DropTable(
                name: "Volunteers");

            migrationBuilder.DropTable(
                name: "Partners");
        }
    }
}
