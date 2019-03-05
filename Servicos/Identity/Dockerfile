FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50519
EXPOSE 44338

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Identity/Identity.API.csproj Identity/
COPY Mensagens/Mensagens.csproj Mensagens/
RUN dotnet restore Identity/Identity.API.csproj
COPY . .
WORKDIR /src/Identity
RUN dotnet build Identity.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Identity.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Identity.API.dll"]
