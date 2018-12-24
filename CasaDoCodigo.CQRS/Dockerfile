FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY CasaDoCodigo.CQRS/MVC.csproj CasaDoCodigo.CQRS/
COPY CasaDoCodigo.Mensagens/CasaDoCodigo.Mensagens.csproj CasaDoCodigo.Mensagens/
RUN dotnet restore CasaDoCodigo.CQRS/MVC.csproj
COPY . .
WORKDIR /src/CasaDoCodigo.CQRS
RUN dotnet build MVC.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish MVC.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MVC.dll"]
