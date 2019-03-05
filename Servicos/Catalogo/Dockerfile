FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50518
EXPOSE 44375

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Catalogo/Catalogo.API.csproj Catalogo/
RUN dotnet restore Catalogo/Catalogo.API.csproj
COPY . .
WORKDIR /src/Catalogo
RUN dotnet build Catalogo.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Catalogo.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Catalogo.API.dll"]
