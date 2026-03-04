using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 4, 7, 40, 23, 513, DateTimeKind.Utc).AddTicks(2921), "$2a$11$.Wzpw92IGhmDhR33OASpzOIK/I5MxOj1XI97xF.4wiOwavY796JJO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 4, 7, 40, 23, 608, DateTimeKind.Utc).AddTicks(8580), "$2a$11$84.bG.cxwDxZcwOawPczH.eNJWo14A81/M7WkYx.gcG940Qb7vnVe" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 4, 7, 40, 23, 708, DateTimeKind.Utc).AddTicks(174), "$2a$11$RmqL614piXEEbflsRVWMqeyCtFP78lSPfzCAnCLxFkpgEJvrxvFkm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "FullName",
                keyValue: null,
                column: "FullName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$4ZXfO5UsvpBNA9Fl8jUeEu/XmriTz3xZXCRI8.GCsLO4nwy5j0ogK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$OdIYJM4VqppAp3DcazL05eIghvh.hj8khQQWoFxQ37d9lz3.s4e9G");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$JCVtCVJVF0iZFjLTLV44wuC6TUM2IJ5dhBnahxwtqU8R5.awjWZNy");
        }
    }
}
