using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartAPI.Migrations
{
    /// <inheritdoc />
    public partial class AlterCartDetailProductVinculation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_detail_product_product_id",
                table: "cart_detail");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropIndex(
                name: "IX_cart_detail_product_id",
                table: "cart_detail");

            migrationBuilder.AlterColumn<long>(
                name: "product_id",
                table: "cart_detail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "product_id",
                table: "cart_detail",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image_url = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_cart_detail_product_id",
                table: "cart_detail",
                column: "product_id");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_detail_product_product_id",
                table: "cart_detail",
                column: "product_id",
                principalTable: "product",
                principalColumn: "id");
        }
    }
}
