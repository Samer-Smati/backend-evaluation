using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PfeProject.Migrations
{
    /// <inheritdoc />
    public partial class FormConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "FormFields",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FormFields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FormFieldOption",
                columns: table => new
                {
                    Label = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormFieldId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormFieldOption", x => new { x.Label, x.Name });
                    table.ForeignKey(
                        name: "FK_FormFieldOption_FormFields_FormFieldId",
                        column: x => x.FormFieldId,
                        principalTable: "FormFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "19dc37e9-5ae5-4721-97c7-3b244620cffa", null, "HR", "HR" },
                    { "5c08ab24-47b6-425e-b832-e67cba9bf57d", null, "Employee", "EMPLOYEE" },
                    { "61a04417-9544-4d8c-9151-d077d5df6818", null, "Manager", "MANAGER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormFieldOption_FormFieldId",
                table: "FormFieldOption",
                column: "FormFieldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormFieldOption");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "19dc37e9-5ae5-4721-97c7-3b244620cffa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5c08ab24-47b6-425e-b832-e67cba9bf57d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "61a04417-9544-4d8c-9151-d077d5df6818");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "FormFields");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "FormFields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
    }
}
