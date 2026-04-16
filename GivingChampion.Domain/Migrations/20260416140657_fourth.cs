using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class fourth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProfileId",
                table: "Reviews",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Profiles_ProfileId",
                table: "Reviews",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Profiles_ProfileId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ProfileId",
                table: "Reviews");
        }
    }
}
