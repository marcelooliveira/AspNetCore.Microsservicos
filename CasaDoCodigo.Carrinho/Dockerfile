#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see http://aka.ms/containercompat 

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 50520
EXPOSE 44332

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY CasaDoCodigo.Carrinho/Carrinho.API.csproj CasaDoCodigo.Carrinho/
COPY CasaDoCodigo.Mensagens/CasaDoCodigo.Mensagens.csproj CasaDoCodigo.Mensagens/
RUN dotnet restore CasaDoCodigo.Carrinho/Carrinho.API.csproj
COPY . .
WORKDIR /src/CasaDoCodigo.Carrinho
RUN dotnet build Carrinho.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Carrinho.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Carrinho.API.dll"]
