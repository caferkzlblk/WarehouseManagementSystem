using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusIDToShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Shipments");

            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "Shipments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_StatusID",
                table: "Shipments",
                column: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Statuses_StatusID",
                table: "Shipments",
                column: "StatusID",
                principalTable: "Statuses",
                principalColumn: "StatusID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Statuses_StatusID",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_StatusID",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "Shipments");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Shipments",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
