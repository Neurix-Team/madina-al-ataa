using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class _10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Volunteers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Volunteers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Partners",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Partners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Certificates",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Certificates",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Volunteers");

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

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Certificates");
        }
    }
}
