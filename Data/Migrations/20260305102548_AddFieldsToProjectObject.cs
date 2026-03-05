using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToProjectObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectObjectId",
                table: "WorkReports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProjectObjects",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProjectObjects",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProjectObjects",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ProjectObjects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 5, 10, 25, 48, 51, DateTimeKind.Utc).AddTicks(5889), "", null });

            migrationBuilder.UpdateData(
                table: "ProjectObjects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 5, 10, 25, 48, 51, DateTimeKind.Utc).AddTicks(6359), "", null });

            migrationBuilder.UpdateData(
                table: "ProjectObjects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 5, 10, 25, 48, 51, DateTimeKind.Utc).AddTicks(6360), "", null });

            migrationBuilder.UpdateData(
                table: "ProjectObjects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 5, 10, 25, 48, 51, DateTimeKind.Utc).AddTicks(6361), "", null });

            migrationBuilder.UpdateData(
                table: "ProjectObjects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Description", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 5, 10, 25, 48, 51, DateTimeKind.Utc).AddTicks(6361), "", null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 5, 10, 25, 47, 861, DateTimeKind.Utc).AddTicks(9206), "$2a$11$g48k0FLjr35yH9n72oilOeJSSdQFHAWc8P8JHwanbqXem8s6bfTwO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 5, 10, 25, 47, 956, DateTimeKind.Utc).AddTicks(1806), "$2a$11$nd/0WUq2smgijqApwRNQQuJOMPh1XO/t57DDcHkPlHYEB1HN7iNHG" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 5, 10, 25, 48, 50, DateTimeKind.Utc).AddTicks(4397), "$2a$11$jYHlxkABeVp1ZD5m3O4Z6ePbRz8/nvFYNPOBs.iwPPZgiqivCgvSq" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkReports_ProjectObjectId",
                table: "WorkReports",
                column: "ProjectObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkReports_ProjectObjects_ProjectObjectId",
                table: "WorkReports",
                column: "ProjectObjectId",
                principalTable: "ProjectObjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkReports_ProjectObjects_ProjectObjectId",
                table: "WorkReports");

            migrationBuilder.DropIndex(
                name: "IX_WorkReports_ProjectObjectId",
                table: "WorkReports");

            migrationBuilder.DropColumn(
                name: "ProjectObjectId",
                table: "WorkReports");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProjectObjects");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProjectObjects");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProjectObjects");

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
    }
}
