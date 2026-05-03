using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class updateUserLevelRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLevels_Profiles_ProfileId",
                table: "UserLevels");

            migrationBuilder.DropIndex(
                name: "IX_UserLevels_ProfileId",
                table: "UserLevels");

            migrationBuilder.CreateIndex(
                name: "IX_UserLevels_ProfileId",
                table: "UserLevels",
                column: "ProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevels_Profiles_ProfileId",
                table: "UserLevels",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLevels_Profiles_ProfileId",
                table: "UserLevels");

            migrationBuilder.DropIndex(
                name: "IX_UserLevels_ProfileId",
                table: "UserLevels");

            migrationBuilder.CreateIndex(
                name: "IX_UserLevels_ProfileId",
                table: "UserLevels",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevels_Profiles_ProfileId",
                table: "UserLevels",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
