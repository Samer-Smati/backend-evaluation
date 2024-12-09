using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PfeProject.Migrations
{
    /// <inheritdoc />
    public partial class rolesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2e4e0c9c-1a98-4023-9e5c-52be2e55eeb2", null, "HR", "HR" },
                    { "3892dd9c-0fe6-45d5-b118-a584a4225afa", null, "Manager", "MANAGER" },
                    { "4afd55c4-c0e6-4ca4-8b2b-177475463763", null, "Employee", "EMPLOYEE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2e4e0c9c-1a98-4023-9e5c-52be2e55eeb2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3892dd9c-0fe6-45d5-b118-a584a4225afa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4afd55c4-c0e6-4ca4-8b2b-177475463763");
        }
    }
}
