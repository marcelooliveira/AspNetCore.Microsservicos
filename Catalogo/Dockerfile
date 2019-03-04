FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50518
EXPOSE 44375

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY CasaDoCodigo.Catalogo/Catalogo.API.csproj CasaDoCodigo.Catalogo/
RUN dotnet restore CasaDoCodigo.Catalogo/Catalogo.API.csproj
COPY . .
WORKDIR /src/CasaDoCodigo.Catalogo
RUN dotnet build Catalogo.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Catalogo.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Catalogo.API.dll"]
