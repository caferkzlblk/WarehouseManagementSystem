using Microsoft.EntityFrameworkCore.Migrations;

namespace WarehouseManagementSystem.Migrations
{
    public partial class CorrectForeignKeyConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderNumbers_OrderNumberID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderNumberID",
                table: "Orders");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderNumbers_OrderNumberID",
                table: "Orders",
                column: "OrderNumberID",
                principalTable: "OrderNumbers",
                principalColumn: "OrderNumberID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
