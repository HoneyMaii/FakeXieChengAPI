using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeXieCheng.API.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d2424642-41d3-43fe-ab75-f6b6db1adac9", "AQAAAAIAAYagAAAAEFO8uWfous7zEpDIK7RufNy8Ps0tfmsL1Vvi0aJ6jaVw6ZTozSIRXk9nM9RUpOn4qA==", "8c226af2-9724-4ddc-b13e-ad4a413191eb" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "beed67cb-9786-4709-94a2-97c5eff658f1", "AQAAAAIAAYagAAAAEGEPdUWEUYsseP8o/Uid5L768vriNDWmn+PgqBoJytxvFhM/vrAm+L2Wy7dmQ7oILA==", "8962bfa3-e156-4793-8784-8366f1a8cc6f" });
        }
    }
}
