#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 50518
EXPOSE 44375

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["CasaDoCodigo.Catalogo/Catalogo.API.csproj", "CasaDoCodigo.Catalogo/"]
RUN dotnet restore "CasaDoCodigo.Catalogo/Catalogo.API.csproj"
COPY . .
WORKDIR "/src/CasaDoCodigo.Catalogo"
RUN dotnet build "Catalogo.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Catalogo.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Catalogo.API.dll"]