# AspNetCore.Microsservicos

## _Instruções de Instalação_

![enter image description here](https://raw.githubusercontent.com/marcelooliveira/AspNetCore.Microsservicos/master/README/arquitetura.png)

### Rodando com o Docker:

 1. Instale o Erlang (linguagem para aplicações distribuídas)
    https://www.rabbitmq.com/which-erlang.html
 2. Instale o RabbitMQ (componente intermediário de mensageria)
    https://www.rabbitmq.com/download.html
 3. Instale o Redis (cache e banco de dados em memória)
    https://redis.io/download
 4. Instale o Visual Studio 2017:
    https://visualstudio.microsoft.com/pt-br/downloads
 5. Instale o Docker (contêiner para distribuição de aplicações)
    https://docs.docker.com/docker-for-windows/install/
    Abra a solução Visual Studio: `AspNetCore.Microsservicos.sln`
6. Defina o projeto de inicialização: `docker-compose.dcproj`
7. Rode a aplicação

### Rodando sem o Docker:

Modifique os passos acima: 

6. Clique com o botão direito sobre o projeto `docker-compose.dcproj`, selecione "descarregar projeto"
7. Defina os seguintes projetos de inicialização:
	
	 - Catalogo/Catalogo.API.csproj
	 - Carrinho/Carrinho.API.csproj
	 - Identity/Identity.API.csproj
	 - OrdemDeCompra/OrdemDeCompra.API.csproj
	 - WebApps/MVC.csproj
	 - WebApps/WebStatus.csproj
	
 8. Rode a aplicação
