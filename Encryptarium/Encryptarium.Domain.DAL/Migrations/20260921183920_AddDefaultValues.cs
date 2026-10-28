using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Encryptarium.Domain.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { 1, "1", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "cFunctionalRuleSecrets",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Функциональное правило View", "View" },
                    { 2, "Функциональное правило Update", "Update" },
                    { 3, "Функциональное правило Add", "Add" },
                    { 4, "Функциональное правило Delete", "Delete" }
                });

            migrationBuilder.InsertData(
                table: "cFunctionalRuleStorages",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Функциональное правило View", "View" },
                    { 2, "Функциональное правило Update", "Update" },
                    { 3, "Функциональное правило Add", "Add" },
                    { 4, "Функциональное правило Delete", "Delete" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "cFunctionalRuleSecrets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "cFunctionalRuleSecrets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "cFunctionalRuleSecrets",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "cFunctionalRuleSecrets",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "cFunctionalRuleStorages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "cFunctionalRuleStorages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "cFunctionalRuleStorages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "cFunctionalRuleStorages",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
