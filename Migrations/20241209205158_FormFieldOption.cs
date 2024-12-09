using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PfeProject.Migrations
{
    /// <inheritdoc />
    public partial class FormFieldOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FormFieldOption",
                table: "FormFieldOption");

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

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FormFieldOption",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "FormFieldOption",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "FormFieldOption",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormFieldOption",
                table: "FormFieldOption",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FormSubmissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FormId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FieldValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormConfigurationId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSubmissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormSubmissions_FormConfigurations_FormConfigurationId",
                        column: x => x.FormConfigurationId,
                        principalTable: "FormConfigurations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormSubmissions_FormConfigurations_FormId",
                        column: x => x.FormId,
                        principalTable: "FormConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_FormConfigurationId",
                table: "FormSubmissions",
                column: "FormConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_FormId",
                table: "FormSubmissions",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_UserId",
                table: "FormSubmissions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormSubmissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormFieldOption",
                table: "FormFieldOption");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FormFieldOption");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FormFieldOption",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "FormFieldOption",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormFieldOption",
                table: "FormFieldOption",
                columns: new[] { "Label", "Name" });

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
    }
}
