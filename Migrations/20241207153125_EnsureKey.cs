using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PfeProject.Migrations
{
    /// <inheritdoc />
    public partial class EnsureKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0f155a6e-301e-458b-9de2-f767deca50da");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9cb6470b-e7ed-446d-9e5c-62359c91fe3c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c37191d6-bfad-417c-ab19-0a2059968b08");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "0f155a6e-301e-458b-9de2-f767deca50da", null, "Manager", "MANAGER" },
                    { "9cb6470b-e7ed-446d-9e5c-62359c91fe3c", null, "Employee", "SIMPLEUSER" },
                    { "c37191d6-bfad-417c-ab19-0a2059968b08", null, "HR", "HR" }
                });
        }
    }
}
