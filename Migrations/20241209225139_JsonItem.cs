using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PfeProject.Migrations
{
    /// <inheritdoc />
    public partial class JsonItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8247d970-c0bf-4705-851c-b6a34d508e14", null, "HR", "HR" },
                    { "b8ad4327-909b-4727-a6b4-99194eafe334", null, "Manager", "MANAGER" },
                    { "f74dcd53-d0de-4fba-a13f-24e7ea7058ee", null, "Employee", "EMPLOYEE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8247d970-c0bf-4705-851c-b6a34d508e14");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b8ad4327-909b-4727-a6b4-99194eafe334");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f74dcd53-d0de-4fba-a13f-24e7ea7058ee");

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
    }
}
