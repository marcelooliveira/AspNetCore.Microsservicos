class Catalogo {
    clickAdicionarAoCarrinho(el) {
        const btn = $(el);
        btn.attr('disabled', true);
        btn.removeClass('btn-success');
        btn.addClass('btn-secondary');
        let codigo = btn.attr('code');
        this.adicionarAoCarrinho(codigo);
    }

    adicionarAoCarrinho(codigo) {
        let token = $('[name=__RequestVerificationToken]').val();

        let headers = {};
        headers['RequestVerificationToken'] = token;

        $.ajax({
            url: '/carrinho/adicionaraocarrinho',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(codigo),
            headers: headers
        }).done(function (response) {
            catalogo.showSnackbar();
        });
    }

    showSnackbar() {
        var x = document.getElementById("snackbar");
        x.className = "show";
        setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
    }
}

var catalogo = new Catalogo();
