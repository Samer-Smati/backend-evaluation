using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PfeProject.Migrations
{
    /// <inheritdoc />
    public partial class roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "18aefdc8-378f-4949-9c3a-b55331bfca7e", null, "Employee", "EMPLOYEE" },
                    { "72fa6d18-6015-4e54-86c2-7646b73db9dd", null, "HR", "HR" },
                    { "bab70191-8eda-4786-be3b-f8aa48ca4f53", null, "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18aefdc8-378f-4949-9c3a-b55331bfca7e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "72fa6d18-6015-4e54-86c2-7646b73db9dd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bab70191-8eda-4786-be3b-f8aa48ca4f53");
        }
    }
}
