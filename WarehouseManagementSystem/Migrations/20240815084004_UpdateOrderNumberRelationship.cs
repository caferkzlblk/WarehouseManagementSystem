using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderNumberRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderNumbers_OrderNumberID",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumberID",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OrderID",
                table: "OrderNumbers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderNumbers_OrderNumberID",
                table: "Orders",
                column: "OrderNumberID",
                principalTable: "OrderNumbers",
                principalColumn: "OrderNumberID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderNumbers_OrderNumberID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderID",
                table: "OrderNumbers");

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumberID",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderNumbers_OrderNumberID",
                table: "Orders",
                column: "OrderNumberID",
                principalTable: "OrderNumbers",
                principalColumn: "OrderNumberID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
