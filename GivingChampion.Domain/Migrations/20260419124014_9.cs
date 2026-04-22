using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class _9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Children",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "Children",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Children",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Children",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Children",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Children_ApprovedById",
                table: "Children",
                column: "ApprovedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Children_AspNetUsers_ApprovedById",
                table: "Children",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Children_AspNetUsers_ApprovedById",
                table: "Children");

            migrationBuilder.DropIndex(
                name: "IX_Children_ApprovedById",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Children");
        }
    }
}
