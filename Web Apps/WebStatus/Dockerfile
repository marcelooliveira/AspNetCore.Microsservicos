FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Status/WebStatus.csproj", "Status/"]
RUN dotnet restore "Status/WebStatus.csproj"
COPY . .
WORKDIR "/src/Status"
RUN dotnet build "WebStatus.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebStatus.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebStatus.dll"]