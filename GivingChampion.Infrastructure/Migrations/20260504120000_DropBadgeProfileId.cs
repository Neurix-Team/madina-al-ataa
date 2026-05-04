using System;
using GivingChampion.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260504120000_DropBadgeProfileId")]
    public partial class DropBadgeProfileId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Badges_Profiles_ProfileId",
                table: "Badges");

            migrationBuilder.DropIndex(
                name: "IX_Badges_ProfileId",
                table: "Badges");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Badges");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProfileId",
                table: "Badges",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

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
