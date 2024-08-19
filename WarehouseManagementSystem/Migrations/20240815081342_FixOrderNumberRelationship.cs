using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderNumberRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderNumberID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderNumberValue",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderID",
                table: "OrderNumbers");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumberID",
                table: "Orders",
                column: "OrderNumberID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderNumberID",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "OrderNumberValue",
                table: "Orders",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "OrderID",
                table: "OrderNumbers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumberID",
                table: "Orders",
                column: "OrderNumberID");
        }
    }
}
