using Microsoft.EntityFrameworkCore.Migrations;

namespace OrdemDeCompra.API.Migrations
{
    public partial class SemProdutoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProdutoId",
                table: "ItemPedido");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProdutoId",
                table: "ItemPedido",
                nullable: false,
                defaultValue: 0);
        }
    }
}
