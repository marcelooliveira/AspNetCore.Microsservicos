FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 5106
EXPOSE 44398

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY OrdemDeCompra.API/OrdemDeCompra.API.csproj OrdemDeCompra.API/
COPY CasaDoCodigo.Mensagens/CasaDoCodigo.Mensagens.csproj CasaDoCodigo.Mensagens/
RUN dotnet restore OrdemDeCompra.API/OrdemDeCompra.API.csproj
COPY . .
WORKDIR /src/OrdemDeCompra.API
RUN dotnet build OrdemDeCompra.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish OrdemDeCompra.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OrdemDeCompra.API.dll"]
