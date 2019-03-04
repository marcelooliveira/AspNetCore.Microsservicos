using Microsoft.EntityFrameworkCore.Migrations;

namespace OrdemDeCompra.API.Migrations
{
    public partial class SemProdutoPreco : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProdutoPreco",
                table: "ItemPedido");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ProdutoPreco",
                table: "ItemPedido",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
