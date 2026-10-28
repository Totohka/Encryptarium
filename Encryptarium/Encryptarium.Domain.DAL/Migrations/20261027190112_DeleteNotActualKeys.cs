using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Encryptarium.Domain.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeleteNotActualKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecretFileId",
                table: "eSecrets");

            migrationBuilder.DropColumn(
                name: "SecretTextId",
                table: "eSecrets");

            migrationBuilder.DropColumn(
                name: "SecretUserPassId",
                table: "eSecrets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SecretFileId",
                table: "eSecrets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SecretTextId",
                table: "eSecrets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SecretUserPassId",
                table: "eSecrets",
                type: "TEXT",
                nullable: true);
        }
    }
}
