FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["CasaDoCodigo.Status/WebStatus.csproj", "CasaDoCodigo.Status/"]
RUN dotnet restore "CasaDoCodigo.Status/WebStatus.csproj"
COPY . .
WORKDIR "/src/CasaDoCodigo.Status"
RUN dotnet build "WebStatus.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebStatus.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebStatus.dll"]