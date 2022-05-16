FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RBBS_CS/RBBS_CS.csproj", "RBBS_CS/"]
COPY ["Server/src/Org.OpenAPIToolsServer/Org.OpenAPIToolsServer.csproj", "Server/src/Org.OpenAPIToolsServer/"]
COPY ["Models.RBB_CS/Models.RBB_CS.csproj", "Models.RBB_CS/"]
COPY ["Client/src/Org.OpenAPITools/Org.OpenAPITools.csproj", "Client/src/Org.OpenAPITools/"]
COPY ["DAL1.RBB_CS/DAL1.RBB_CS.csproj", "DAL1.RBB_CS/"]
RUN dotnet restore "RBBS_CS/RBBS_CS.csproj"
COPY . .
WORKDIR "/src/RBBS_CS"
RUN dotnet build "RBBS_CS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RBBS_CS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RBBS_CS.dll"]/