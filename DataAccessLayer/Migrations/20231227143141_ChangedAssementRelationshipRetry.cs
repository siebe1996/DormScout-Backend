using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangedAssementRelationshipRetry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Places_PlaceId",
                table: "Assessments");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_PlaceId",
                table: "Assessments");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "Assessments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlaceId",
                table: "Assessments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_PlaceId",
                table: "Assessments",
                column: "PlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Places_PlaceId",
                table: "Assessments",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
