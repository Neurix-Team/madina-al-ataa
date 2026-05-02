using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Updatingprofileandbadgerelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Badges_Profiles_ProfileId",
                table: "Badges");

            migrationBuilder.DropIndex(
                name: "IX_UserBadges_ProfileId",
                table: "UserBadges");

            migrationBuilder.DropIndex(
                name: "IX_Badges_ProfileId",
                table: "Badges");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Badges");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_ProfileId_BadgeId",
                table: "UserBadges",
                columns: new[] { "ProfileId", "BadgeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBadges_ProfileId_BadgeId",
                table: "UserBadges");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileId",
                table: "Badges",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_ProfileId",
                table: "UserBadges",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Badges_ProfileId",
                table: "Badges",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Badges_Profiles_ProfileId",
                table: "Badges",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
