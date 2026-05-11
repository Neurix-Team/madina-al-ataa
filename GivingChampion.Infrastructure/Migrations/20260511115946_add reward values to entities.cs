using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addrewardvaluestoentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImpactReward",
                table: "ServiceRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KPReward",
                table: "ServiceRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "XPReward",
                table: "ServiceRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImpactReward",
                table: "GeoQuests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KPReward",
                table: "GeoQuests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "XPReward",
                table: "GeoQuests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImpactReward",
                table: "DonationRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KPReward",
                table: "DonationRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "XPReward",
                table: "DonationRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Requirement",
                table: "Badges",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RewardTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceType = table.Column<int>(type: "integer", nullable: false),
                    SourceEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    XPAmount = table.Column<int>(type: "integer", nullable: false),
                    KPAmount = table.Column<int>(type: "integer", nullable: false),
                    ImpactAmount = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    EarnedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RewardTransactions_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RewardTransactions_ProfileId_SourceType_ActionEntityId",
                table: "RewardTransactions",
                columns: new[] { "ProfileId", "SourceType", "ActionEntityId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RewardTransactions");

            migrationBuilder.DropColumn(
                name: "ImpactReward",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "KPReward",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "XPReward",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ImpactReward",
                table: "GeoQuests");

            migrationBuilder.DropColumn(
                name: "KPReward",
                table: "GeoQuests");

            migrationBuilder.DropColumn(
                name: "XPReward",
                table: "GeoQuests");

            migrationBuilder.DropColumn(
                name: "ImpactReward",
                table: "DonationRequests");

            migrationBuilder.DropColumn(
                name: "KPReward",
                table: "DonationRequests");

            migrationBuilder.DropColumn(
                name: "XPReward",
                table: "DonationRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Requirement",
                table: "Badges",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
