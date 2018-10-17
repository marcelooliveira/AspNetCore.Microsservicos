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
            Id: itemId,
            ProdutoId: itemId,
            Quantidade: novaQuantidade
        };
    }

    postQuantidade(data) {

        let token = $('[name=__RequestVerificationToken]').val();

        let headers = {};
        headers['RequestVerificationToken'] = token;

        $.ajax({
            url: '/pedido/updatequantidade',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            headers: headers
        }).done(function (response) {
            var itens = response.Itens;
            let itemPedido;
            for (var i = 0; i < itens.length; i++) {
                if (itens[i].ProdutoId === data.ProdutoId) {
                    itemPedido = itens[i];
                    break;
                }
            }
            let linhaDoItem = $('[item-id=' + itemPedido.ProdutoId + ']')
            linhaDoItem.find('input').val(itemPedido.Quantidade);
            linhaDoItem.find('[subtotal]').html((itemPedido.Subtotal).duasCasas());
            //let carrinhoViewModel = response.carrinhoViewModel;
            //$('[numero-itens]').html('Total: ' + carrinhoViewModel.itens.length + ' itens');
            //$('[total]').html((carrinhoViewModel.total).duasCasas());
            debugger;

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
