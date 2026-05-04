using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GivingChampion.Domain.Migrations
{
    /// <inheritdoc />
    public partial class moveentityrelationshipttodbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avatars_Profiles_ProfileId",
                table: "Avatars");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Volunteers_IssuedTo",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Children_AspNetUsers_ApprovedById",
                table: "Children");

            migrationBuilder.DropForeignKey(
                name: "FK_Children_AspNetUsers_ParentId",
                table: "Children");

            migrationBuilder.DropForeignKey(
                name: "FK_Children_AspNetUsers_UserId",
                table: "Children");

            migrationBuilder.DropForeignKey(
                name: "FK_DonationOrders_AspNetUsers_DonorId",
                table: "DonationOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_DonationOrders_DonationRequests_DonationRequestId",
                table: "DonationOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_DonationRequests_Locations_LocationId",
                table: "DonationRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_DonationRequests_Partners_PartnerId",
                table: "DonationRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Donors_AspNetUsers_UserId",
                table: "Donors");

            migrationBuilder.DropForeignKey(
                name: "FK_GeoQuests_Locations_LocationId",
                table: "GeoQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_Missions_Locations_LocationId",
                table: "Missions");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_AspNetUsers_UserId",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Levels_LevelId",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_ReviewerId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_AspNetUsers_VolunteerUserId",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Locations_LocationId",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Partners_PartnerId",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGeoQuests_AspNetUsers_UserId",
                table: "UserGeoQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGeoQuests_GeoQuests_GeoQuestId",
                table: "UserGeoQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLevels_Levels_LevelId",
                table: "UserLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLevels_Profiles_ProfileId",
                table: "UserLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMissions_AspNetUsers_UserId",
                table: "UserMissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMissions_Missions_MissionId",
                table: "UserMissions");

            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerOrders_AspNetUsers_UserId",
                table: "VolunteerOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerOrders_ServiceRequests_ServiceRequestId",
                table: "VolunteerOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_AspNetUsers_UserId",
                table: "Volunteers");

            migrationBuilder.DropIndex(
                name: "IX_Volunteers_UserId",
                table: "Volunteers");

            migrationBuilder.DropIndex(
                name: "IX_UserMissions_UserId",
                table: "UserMissions");

            migrationBuilder.DropIndex(
                name: "IX_UserGeoQuests_UserId",
                table: "UserGeoQuests");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_UserId",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Donors_UserId",
                table: "Donors");

            migrationBuilder.DropIndex(
                name: "IX_Children_UserId",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "VolunteerId",
                table: "Certificates");

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_UserId",
                table: "Volunteers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHistories_ServiceRequestId",
                table: "VolunteerHistories",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHistories_UserId",
                table: "VolunteerHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHistories_VolunteerOrderId",
                table: "VolunteerHistories",
                column: "VolunteerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMissions_UserId_MissionId",
                table: "UserMissions",
                columns: new[] { "UserId", "MissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGeoQuests_UserId_GeoQuestId",
                table: "UserGeoQuests",
                columns: new[] { "UserId", "GeoQuestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                table: "Profiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donors_UserId",
                table: "Donors",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Children_UserId",
                table: "Children",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Avatars_Profiles_ProfileId",
                table: "Avatars",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Volunteers_IssuedTo",
                table: "Certificates",
                column: "IssuedTo",
                principalTable: "Volunteers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Children_AspNetUsers_ApprovedById",
                table: "Children",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Children_AspNetUsers_ParentId",
                table: "Children",
                column: "ParentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Children_AspNetUsers_UserId",
                table: "Children",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DonationOrders_AspNetUsers_DonorId",
                table: "DonationOrders",
                column: "DonorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DonationOrders_DonationRequests_DonationRequestId",
                table: "DonationOrders",
                column: "DonationRequestId",
                principalTable: "DonationRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DonationRequests_Locations_LocationId",
                table: "DonationRequests",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DonationRequests_Partners_PartnerId",
                table: "DonationRequests",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Donors_AspNetUsers_UserId",
                table: "Donors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GeoQuests_Locations_LocationId",
                table: "GeoQuests",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_Locations_LocationId",
                table: "Missions",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_AspNetUsers_UserId",
                table: "Profiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Levels_LevelId",
                table: "Profiles",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_ReviewerId",
                table: "Reviews",
                column: "ReviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_AspNetUsers_VolunteerUserId",
                table: "ServiceRequests",
                column: "VolunteerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Locations_LocationId",
                table: "ServiceRequests",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Partners_PartnerId",
                table: "ServiceRequests",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGeoQuests_AspNetUsers_UserId",
                table: "UserGeoQuests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGeoQuests_GeoQuests_GeoQuestId",
                table: "UserGeoQuests",
                column: "GeoQuestId",
                principalTable: "GeoQuests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevels_Levels_LevelId",
                table: "UserLevels",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevels_Profiles_ProfileId",
                table: "UserLevels",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMissions_AspNetUsers_UserId",
                table: "UserMissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMissions_Missions_MissionId",
                table: "UserMissions",
                column: "MissionId",
                principalTable: "Missions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHistories_AspNetUsers_UserId",
                table: "VolunteerHistories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHistories_ServiceRequests_ServiceRequestId",
                table: "VolunteerHistories",
                column: "ServiceRequestId",
                principalTable: "ServiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHistories_VolunteerOrders_VolunteerOrderId",
                table: "VolunteerHistories",
                column: "VolunteerOrderId",
                principalTable: "VolunteerOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerOrders_AspNetUsers_UserId",
                table: "VolunteerOrders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerOrders_ServiceRequests_ServiceRequestId",
                table: "VolunteerOrders",
                column: "ServiceRequestId",
                principalTable: "ServiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_AspNetUsers_UserId",
                table: "Volunteers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avatars_Profiles_ProfileId",
                table: "Avatars");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Volunteers_IssuedTo",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Children_AspNetUsers_ApprovedById",
                table: "Children");

            migrationBuilder.DropForeignKey(
                name: "FK_Children_AspNetUsers_ParentId",
                table: "Children");

            migrationBuilder.DropForeignKey(
                name: "FK_Children_AspNetUsers_UserId",
                table: "Children");

            migrationBuilder.DropForeignKey(
                name: "FK_DonationOrders_AspNetUsers_DonorId",
                table: "DonationOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_DonationOrders_DonationRequests_DonationRequestId",
                table: "DonationOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_DonationRequests_Locations_LocationId",
                table: "DonationRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_DonationRequests_Partners_PartnerId",
                table: "DonationRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Donors_AspNetUsers_UserId",
                table: "Donors");

            migrationBuilder.DropForeignKey(
                name: "FK_GeoQuests_Locations_LocationId",
                table: "GeoQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_Missions_Locations_LocationId",
                table: "Missions");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_AspNetUsers_UserId",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Levels_LevelId",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_ReviewerId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_AspNetUsers_VolunteerUserId",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Locations_LocationId",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Partners_PartnerId",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGeoQuests_AspNetUsers_UserId",
                table: "UserGeoQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGeoQuests_GeoQuests_GeoQuestId",
                table: "UserGeoQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLevels_Levels_LevelId",
                table: "UserLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLevels_Profiles_ProfileId",
                table: "UserLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMissions_AspNetUsers_UserId",
                table: "UserMissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMissions_Missions_MissionId",
                table: "UserMissions");

            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHistories_AspNetUsers_UserId",
                table: "VolunteerHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHistories_ServiceRequests_ServiceRequestId",
                table: "VolunteerHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHistories_VolunteerOrders_VolunteerOrderId",
                table: "VolunteerHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerOrders_AspNetUsers_UserId",
                table: "VolunteerOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerOrders_ServiceRequests_ServiceRequestId",
                table: "VolunteerOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_AspNetUsers_UserId",
                table: "Volunteers");

            migrationBuilder.DropIndex(
                name: "IX_Volunteers_UserId",
                table: "Volunteers");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerHistories_ServiceRequestId",
                table: "VolunteerHistories");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerHistories_UserId",
                table: "VolunteerHistories");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerHistories_VolunteerOrderId",
                table: "VolunteerHistories");

            migrationBuilder.DropIndex(
                name: "IX_UserMissions_UserId_MissionId",
                table: "UserMissions");

            migrationBuilder.DropIndex(
                name: "IX_UserGeoQuests_UserId_GeoQuestId",
                table: "UserGeoQuests");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_UserId",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Donors_UserId",
                table: "Donors");

            migrationBuilder.DropIndex(
                name: "IX_Children_UserId",
                table: "Children");

            migrationBuilder.AddColumn<Guid>(
                name: "VolunteerId",
                table: "Certificates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_UserId",
                table: "Volunteers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMissions_UserId",
                table: "UserMissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGeoQuests_UserId",
                table: "UserGeoQuests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                table: "Profiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Donors_UserId",
                table: "Donors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Children_UserId",
                table: "Children",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Avatars_Profiles_ProfileId",
                table: "Avatars",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Volunteers_IssuedTo",
                table: "Certificates",
                column: "IssuedTo",
                principalTable: "Volunteers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Children_AspNetUsers_ApprovedById",
                table: "Children",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Children_AspNetUsers_ParentId",
                table: "Children",
                column: "ParentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Children_AspNetUsers_UserId",
                table: "Children",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonationOrders_AspNetUsers_DonorId",
                table: "DonationOrders",
                column: "DonorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonationOrders_DonationRequests_DonationRequestId",
                table: "DonationOrders",
                column: "DonationRequestId",
                principalTable: "DonationRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonationRequests_Locations_LocationId",
                table: "DonationRequests",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonationRequests_Partners_PartnerId",
                table: "DonationRequests",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Donors_AspNetUsers_UserId",
                table: "Donors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GeoQuests_Locations_LocationId",
                table: "GeoQuests",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_Locations_LocationId",
                table: "Missions",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_AspNetUsers_UserId",
                table: "Profiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Levels_LevelId",
                table: "Profiles",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_ReviewerId",
                table: "Reviews",
                column: "ReviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_AspNetUsers_VolunteerUserId",
                table: "ServiceRequests",
                column: "VolunteerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Locations_LocationId",
                table: "ServiceRequests",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Partners_PartnerId",
                table: "ServiceRequests",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGeoQuests_AspNetUsers_UserId",
                table: "UserGeoQuests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGeoQuests_GeoQuests_GeoQuestId",
                table: "UserGeoQuests",
                column: "GeoQuestId",
                principalTable: "GeoQuests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevels_Levels_LevelId",
                table: "UserLevels",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevels_Profiles_ProfileId",
                table: "UserLevels",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMissions_AspNetUsers_UserId",
                table: "UserMissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMissions_Missions_MissionId",
                table: "UserMissions",
                column: "MissionId",
                principalTable: "Missions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerOrders_AspNetUsers_UserId",
                table: "VolunteerOrders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerOrders_ServiceRequests_ServiceRequestId",
                table: "VolunteerOrders",
                column: "ServiceRequestId",
                principalTable: "ServiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_AspNetUsers_UserId",
                table: "Volunteers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
