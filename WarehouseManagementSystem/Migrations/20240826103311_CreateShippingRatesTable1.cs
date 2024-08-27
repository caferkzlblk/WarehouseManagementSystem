using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class CreateShippingRatesTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingRates",
                columns: table => new
                {
                    ShippingRateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShippingCompanyID = table.Column<int>(type: "int", nullable: false),
                    MinProductCount = table.Column<int>(type: "int", nullable: false),
                    MaxProductCount = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRates", x => x.ShippingRateID);
                    table.ForeignKey(
                        name: "FK_ShippingRates_ShippingCompanies_ShippingCompanyID",
                        column: x => x.ShippingCompanyID,
                        principalTable: "ShippingCompanies",
                        principalColumn: "ShippingCompanyID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_ShippingCompanyID",
                table: "ShippingRates",
                column: "ShippingCompanyID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRates");
        }
    }
}
