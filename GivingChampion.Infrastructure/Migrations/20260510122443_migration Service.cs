using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class migrationService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxOrders",
                table: "ServiceRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RequiredLevelId",
                table: "ServiceRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_RequiredLevelId",
                table: "ServiceRequests",
                column: "RequiredLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Levels_RequiredLevelId",
                table: "ServiceRequests",
                column: "RequiredLevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Levels_RequiredLevelId",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_RequiredLevelId",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "MaxOrders",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "RequiredLevelId",
                table: "ServiceRequests");
        }
    }
}
