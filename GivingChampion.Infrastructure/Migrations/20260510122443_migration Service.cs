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
                nullable: true);

            migrationBuilder.Sql(
                """
                INSERT INTO "Levels" ("Id", "Number", "MaxXp", "CreatedAt", "UpdatedAt", "IsDeleted", "DeletedAt")
                SELECT '11111111-1111-1111-1111-111111111111'::uuid, 1, 100, NOW() AT TIME ZONE 'UTC', TIMESTAMP '0001-01-01 00:00:00', FALSE, NULL
                WHERE NOT EXISTS (SELECT 1 FROM "Levels");
                """);

            migrationBuilder.Sql(
                """
                UPDATE "ServiceRequests"
                SET "RequiredLevelId" = COALESCE(
                    (SELECT "Id" FROM "Levels" WHERE NOT "IsDeleted" ORDER BY "Number", "CreatedAt" LIMIT 1),
                    (SELECT "Id" FROM "Levels" ORDER BY "Number", "CreatedAt" LIMIT 1)
                )
                WHERE "RequiredLevelId" IS NULL;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "RequiredLevelId",
                table: "ServiceRequests",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

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
