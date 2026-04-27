using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class _15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonationOrders_Donors_DonorId",
                table: "DonationOrders");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "DonationRequests");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "DonationRequests");

            migrationBuilder.DropColumn(
                name: "ImpactReport",
                table: "DonationOrders");

            migrationBuilder.DropColumn(
                name: "TargetLocation",
                table: "DonationOrders");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "DonationRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "DonationRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DonationRequests_LocationId",
                table: "DonationRequests",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DonationOrders_AspNetUsers_DonorId",
                table: "DonationOrders",
                column: "DonorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonationRequests_Locations_LocationId",
                table: "DonationRequests",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonationOrders_AspNetUsers_DonorId",
                table: "DonationOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_DonationRequests_Locations_LocationId",
                table: "DonationRequests");

            migrationBuilder.DropIndex(
                name: "IX_DonationRequests_LocationId",
                table: "DonationRequests");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "DonationRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "DonationRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "DonationRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "DonationRequests",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImpactReport",
                table: "DonationOrders",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetLocation",
                table: "DonationOrders",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_DonationOrders_Donors_DonorId",
                table: "DonationOrders",
                column: "DonorId",
                principalTable: "Donors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
