FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY CasaDoCodigo.Carrinho/Carrinho.API.csproj CasaDoCodigo.Carrinho/
COPY CasaDoCodigo.Mensagens/CasaDoCodigo.Mensagens.csproj CasaDoCodigo.Mensagens/
RUN dotnet restore CasaDoCodigo.Carrinho/Carrinho.API.csproj
COPY . .
WORKDIR /src/CasaDoCodigo.Carrinho
RUN dotnet build Carrinho.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Carrinho.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Carrinho.API.dll"]
