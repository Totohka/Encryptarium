using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Encryptarium.Domain.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixIncorrectNameField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_xUserRoles_eRoles_eRoleId",
                table: "xUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_xUserRoles_eRoleId",
                table: "xUserRoles");

            migrationBuilder.DropColumn(
                name: "eRoleId",
                table: "xUserRoles");

            migrationBuilder.CreateIndex(
                name: "IX_xUserRoles_RoleId",
                table: "xUserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_xUserRoles_eRoles_RoleId",
                table: "xUserRoles",
                column: "RoleId",
                principalTable: "eRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_xUserRoles_eRoles_RoleId",
                table: "xUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_xUserRoles_RoleId",
                table: "xUserRoles");

            migrationBuilder.AddColumn<int>(
                name: "eRoleId",
                table: "xUserRoles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_xUserRoles_eRoleId",
                table: "xUserRoles",
                column: "eRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_xUserRoles_eRoles_eRoleId",
                table: "xUserRoles",
                column: "eRoleId",
                principalTable: "eRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
