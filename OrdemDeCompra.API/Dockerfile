#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see http://aka.ms/containercompat 

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 50516
EXPOSE 44398

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
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
