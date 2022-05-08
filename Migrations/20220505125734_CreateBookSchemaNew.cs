using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDev1.Migrations
{
    public partial class CreateBookSchemaNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cart_UserId",
                table: "Cart");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_BookIsbn",
                table: "Cart",
                column: "BookIsbn");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Book_BookIsbn",
                table: "Cart",
                column: "BookIsbn",
                principalTable: "Book",
                principalColumn: "BookIsbn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Book_BookIsbn",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_BookIsbn",
                table: "Cart");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserId",
                table: "Cart",
                column: "UserId",
                unique: true);
        }
    }
}
