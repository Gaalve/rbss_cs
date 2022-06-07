#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RBSS_CS/RBSS_CS.csproj", "RBSS_CS/"]
COPY ["Server/src/Org.OpenAPIToolsServer/Org.OpenAPIToolsServer.csproj", "Server/src/Org.OpenAPIToolsServer/"]
COPY ["Models.RBSS_CS/Models.RBSS_CS.csproj", "Models.RBSS_CS/"]
COPY ["Client/src/Org.OpenAPITools/Org.OpenAPITools.csproj", "Client/src/Org.OpenAPITools/"]
COPY ["DAL1.RBSS_CS/DAL1.RBSS_CS.csproj", "DAL1.RBSS_CS/"]
RUN dotnet restore "RBSS_CS/RBSS_CS.csproj"
COPY . .
WORKDIR "/src/RBSS_CS"
RUN dotnet build "RBSS_CS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RBSS_CS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RBSS_CS.dll"]