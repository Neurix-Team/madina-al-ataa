using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingProfileUserLevelRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Levels_LevelId",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLevels_Levels_LevelId",
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
                name: "FK_Profiles_Levels_LevelId",
                table: "Profiles",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevels_Levels_LevelId",
                table: "UserLevels",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Levels_LevelId",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLevels_Levels_LevelId",
                table: "UserLevels");

            migrationBuilder.DropIndex(
                name: "IX_UserLevels_ProfileId",
                table: "UserLevels");

            migrationBuilder.CreateIndex(
                name: "IX_UserLevels_ProfileId",
                table: "UserLevels",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Levels_LevelId",
                table: "Profiles",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevels_Levels_LevelId",
                table: "UserLevels",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
