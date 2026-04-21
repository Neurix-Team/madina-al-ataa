using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class _11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "VolunteerOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "VolunteerOrders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ServiceRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ServiceRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ServiceRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "VolunteerOrders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "VolunteerOrders");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ServiceRequests");
        }
    }
}
