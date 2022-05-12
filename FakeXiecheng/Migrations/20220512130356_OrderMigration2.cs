using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeXieCheng.API.Migrations
{
    public partial class OrderMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "b7b03c05-f128-4569-a531-f739ead8b58a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "98ed7ab7-2629-44a0-9e85-e2b2a7123d98", "AQAAAAEAACcQAAAAEBuE6Yxq5i6ufrQJtpsW7447u92NREehOXrU3B9I36NaYwzku6c03UNp5P4JF56a/A==", "afe5fed0-3cd0-45ce-afe6-dd2408f57f64" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "081dc89e-d0c2-4d9c-ab8d-0ef3a40c3ca9");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e69fba07-4b11-4e98-9f48-27784f1add28", "AQAAAAEAACcQAAAAEKIHKG5XwiANGH+JnkdxSDNz02ZWNP1X5cqOr4WOVhGY/EVykaAy6PQPlS6sM4k3ug==", "ee078351-5a86-4857-a9f8-fd63e11f0559" });
        }
    }
}
