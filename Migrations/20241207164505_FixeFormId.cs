using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PfeProject.Migrations
{
    /// <inheritdoc />
    public partial class FixeFormId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ed8cbfa-ec49-4e5c-a4d3-280b1d4116dc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c6655b4f-b44b-4dde-a61c-f8812c0c08a5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e8367b10-e579-4912-b68f-453556ea11e1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "15069580-56ca-4545-9eff-e56e688bc3de", null, "Manager", "MANAGER" },
                    { "9694a5ef-cdad-472e-9c10-efef9d9da3aa", null, "HR", "HR" },
                    { "d21d7a19-83a1-4e97-b4c5-a22a8d8cd701", null, "Employee", "EMPLOYEE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "15069580-56ca-4545-9eff-e56e688bc3de");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9694a5ef-cdad-472e-9c10-efef9d9da3aa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d21d7a19-83a1-4e97-b4c5-a22a8d8cd701");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5ed8cbfa-ec49-4e5c-a4d3-280b1d4116dc", null, "Employee", "EMPLOYEE" },
                    { "c6655b4f-b44b-4dde-a61c-f8812c0c08a5", null, "Manager", "MANAGER" },
                    { "e8367b10-e579-4912-b68f-453556ea11e1", null, "HR", "HR" }
                });
        }
    }
}
