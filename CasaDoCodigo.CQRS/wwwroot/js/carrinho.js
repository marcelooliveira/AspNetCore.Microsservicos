class Carrinho {
    clickIncremento(button) {
        let data = this.getData(button);
        data.Quantidade++;
        this.postQuantidade(data);
    }

    clickDecremento(button) {
        let data = this.getData(button);
        data.Quantidade--;
        this.postQuantidade(data);
    }

    updateQuantidade(input) {
        let data = this.getData(input);
        this.postQuantidade(data);
    }

    getData(elemento) {
        var linhaDoItem = $(elemento).parents('[item-id]');
        var itemId = $(linhaDoItem).attr('item-id');
        var novaQuantidade = $(linhaDoItem).find('input.quantidade').val();

        return {
            ProdutoId: itemId,
            Quantidade: novaQuantidade
        };
    }

    postQuantidade(data) {

        let token = $('[name=__RequestVerificationToken]').val();

        let headers = {};
        headers['RequestVerificationToken'] = token;

        $.ajax({
            url: '/carrinho/updatequantidade',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            headers: headers
        }).done(function (response) {
            let itemPedido = response.ItemPedido;
            let linhaDoItem = $('[item-id=' + itemPedido.Id + ']');
            linhaDoItem.find('input').val(itemPedido.Quantidade);
            linhaDoItem.find('[subtotal]').html((itemPedido.Subtotal).duasCasas());
            let carrinhoCliente = response.CarrinhoCliente;
            $('[numero-itens]').html('Total: ' + carrinhoCliente.Itens.length + ' itens');
            $('[total]').html((carrinhoCliente.Total).duasCasas());

            if (itemPedido.Quantidade === 0) {
                linhaDoItem.remove();
            }
        });
    }
}

var carrinho = new Carrinho();

Number.prototype.duasCasas = function () {
    return this.toFixed(2).replace('.', ',');
};
