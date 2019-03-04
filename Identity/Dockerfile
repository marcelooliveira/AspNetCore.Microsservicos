FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50519
EXPOSE 44338

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY CasaDoCodigo.Identity/Identity.API.csproj CasaDoCodigo.Identity/
COPY CasaDoCodigo.Mensagens/CasaDoCodigo.Mensagens.csproj CasaDoCodigo.Mensagens/
RUN dotnet restore CasaDoCodigo.Identity/Identity.API.csproj
COPY . .
WORKDIR /src/CasaDoCodigo.Identity
RUN dotnet build Identity.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Identity.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Identity.API.dll"]
