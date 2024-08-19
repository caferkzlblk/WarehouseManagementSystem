using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderNumberValueColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                        name: "OrderNumberValue",
                        table: "OrderNumbers",
                        type: "varchar(255)",
                        nullable: false,
                        defaultValue: "");
            migrationBuilder.AlterColumn<string>(
    name: "OrderNumberValue",
    table: "Orders",
    type: "varchar(255)",
    nullable: true, // Change to false if it should not be nullable
    oldClrType: typeof(string),
    oldType: "varchar(255)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                        name: "OrderNumberValue",
                        table: "OrderNumbers"); 
            migrationBuilder.DropColumn(
                        name: "OrderNumberValue",
                        table: "Order");
        }
    }
}
