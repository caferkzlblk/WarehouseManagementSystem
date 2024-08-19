using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderNumberIDToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderNumberID",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumberID",
                table: "Orders",
                column: "OrderNumberID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderNumbers_OrderNumberID",
                table: "Orders",
                column: "OrderNumberID",
                principalTable: "OrderNumbers",
                principalColumn: "OrderNumberID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderNumbers_OrderNumberID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderNumberID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderNumberID",
                table: "Orders");
        }
    }
}
