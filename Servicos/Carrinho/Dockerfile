FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Carrinho/Carrinho.API.csproj Carrinho/
COPY Mensagens/Mensagens.csproj Mensagens/
RUN dotnet restore Carrinho/Carrinho.API.csproj
COPY . .
WORKDIR /src/Carrinho
RUN dotnet build Carrinho.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Carrinho.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Carrinho.API.dll"]
