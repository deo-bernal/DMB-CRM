# Build from DMB-CRM repo root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DMB.CRM.API/DMB.CRM.API.csproj", "DMB.CRM.API/"]
COPY ["DMB.CRM.Service/DMB.CRM.Service.csproj", "DMB.CRM.Service/"]
COPY ["DMB.CRM.DATA/DMB.CRM.DATA.csproj", "DMB.CRM.DATA/"]
COPY ["DMB.CRM.MODEL/DMB.CRM.MODEL.csproj", "DMB.CRM.MODEL/"]
RUN dotnet restore "DMB.CRM.API/DMB.CRM.API.csproj"
COPY . .
RUN dotnet publish "DMB.CRM.API/DMB.CRM.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
CMD ASPNETCORE_URLS=http://0.0.0.0:$PORT dotnet DMB.CRM.API.dll
