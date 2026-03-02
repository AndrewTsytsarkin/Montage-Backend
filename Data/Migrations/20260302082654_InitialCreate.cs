using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProjectObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectObjects", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Login = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subtype = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Unit = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserObjectAssignments",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ObjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserObjectAssignments", x => new { x.UserId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_UserObjectAssignments_ProjectObjects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "ProjectObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserObjectAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ObjectId = table.Column<int>(type: "int", nullable: false),
                    WorkTypeId = table.Column<int>(type: "int", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Unit = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comment = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PricePerUnit = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkReports_ProjectObjects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "ProjectObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkReports_WorkTypes_WorkTypeId",
                        column: x => x.WorkTypeId,
                        principalTable: "WorkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "ProjectObjects",
                columns: new[] { "Id", "Address", "Name", "Status" },
                values: new object[,]
                {
                    { 1, "г. Москва, Лиговский пр-т, д. 30А", "ТЦ «Галерея» — 3 этаж", "В работе" },
                    { 2, "г. Москва, Пресненская наб., д. 12", "Офисный центр «Сити Плаза»", "В работе" },
                    { 3, "г. Подольск, ул. Индустриальная, д. 45", "Складской комплекс «Логистик»", "Новый" },
                    { 4, "г. Химки, ул. Молодежная, д. 78", "ЖК «Солнечный» — Корпус 5", "Завершен" },
                    { 5, "г. Котельники, мкр. Белая Дача, д. 35", "Ресторан «Белая Дача»", "В работе" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FullName", "Login", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, "Админ Админыч", "admin", "$2a$11$4ZXfO5UsvpBNA9Fl8jUeEu/XmriTz3xZXCRI8.GCsLO4nwy5j0ogK", "Admin" },
                    { 2, "Иванов Д.Д.", "worker1", "$2a$11$OdIYJM4VqppAp3DcazL05eIghvh.hj8khQQWoFxQ37d9lz3.s4e9G", "Worker" },
                    { 3, "Сидоров И.Д.", "worker2", "$2a$11$JCVtCVJVF0iZFjLTLV44wuC6TUM2IJ5dhBnahxwtqU8R5.awjWZNy", "Worker" }
                });

            migrationBuilder.InsertData(
                table: "WorkTypes",
                columns: new[] { "Id", "Name", "PricePerUnit", "SortOrder", "Subtype", "Type", "Unit" },
                values: new object[,]
                {
                    { 1, "Устройство (сверление) сквозных отверстий в стенах из кирпича, железобетонных перекрытиях, сенгвиче, сенгвич-панелях, металлических конструкциях прочих перегородках и стеновых конструкциях диаметром до 22 мм, глубиной до 1м, на высоте до 4м.", 150.00m, 1, "Отверстия", "Сверление, штробление и подрозетники", "шт" },
                    { 2, "Устройство (сверление) сквозных отверстий в стенах из кирпича, железобетонных перекрытиях, сенгвиче, сенгвич-панелях, прочих перегородках и стеновых конструкциях диаметром до 42 мм, глубиной до 1м, на высоте до 4м.", 250.00m, 2, "Отверстия", "Сверление, штробление и подрозетники", "шт" },
                    { 3, "Устройство (сверление) сквозных отверстий в стенах из кирпича, железобетонных перекрытиях, сенгвиче, сенгвич-панелях, прочих перегородках и стеновых конструкциях диаметром до 65 мм, глубиной до 380мм, на высоте до 4м.", 400.00m, 3, "Отверстия", "Сверление, штробление и подрозетники", "шт" },
                    { 4, "Устройство (сверление) сквозных отверстий в металлических конструкциях (швелерах, метал. листах, лотках, крышках, профлистах, перегородках, существующих электрощитах, коммутационных щкафах) диаметром до 25 мм, глубиной до 6мм, на высоте до 4м.", 200.00m, 4, "Отверстия", "Сверление, штробление и подрозетники", "шт" }
                });

            migrationBuilder.InsertData(
                table: "UserObjectAssignments",
                columns: new[] { "ObjectId", "UserId" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 3, 3 },
                    { 4, 3 },
                    { 5, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserObjectAssignments_ObjectId",
                table: "UserObjectAssignments",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkReports_ObjectId_WorkDate",
                table: "WorkReports",
                columns: new[] { "ObjectId", "WorkDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkReports_UserId_WorkDate",
                table: "WorkReports",
                columns: new[] { "UserId", "WorkDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkReports_WorkTypeId",
                table: "WorkReports",
                column: "WorkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTypes_Type_Subtype",
                table: "WorkTypes",
                columns: new[] { "Type", "Subtype" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserObjectAssignments");

            migrationBuilder.DropTable(
                name: "WorkReports");

            migrationBuilder.DropTable(
                name: "ProjectObjects");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkTypes");
        }
    }
}
