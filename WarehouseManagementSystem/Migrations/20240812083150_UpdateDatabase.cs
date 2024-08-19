using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusesStatusID",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusesStatusID1",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_StatusesStatusID",
                table: "OrderDetails",
                column: "StatusesStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_StatusesStatusID1",
                table: "OrderDetails",
                column: "StatusesStatusID1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Statuses_StatusesStatusID",
                table: "OrderDetails",
                column: "StatusesStatusID",
                principalTable: "Statuses",
                principalColumn: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Statuses_StatusesStatusID1",
                table: "OrderDetails",
                column: "StatusesStatusID1",
                principalTable: "Statuses",
                principalColumn: "StatusID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Statuses_StatusesStatusID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Statuses_StatusesStatusID1",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_StatusesStatusID",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_StatusesStatusID1",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "StatusesStatusID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "StatusesStatusID1",
                table: "OrderDetails");
        }
    }
}
