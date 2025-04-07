FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WebApplication/WebApplication.sln .

COPY WebApplication/SmartEstate.API/*.csproj ./SmartEstate.API/
COPY WebApplication/SmartEstate.Application/*.csproj ./SmartEstate.Application/
COPY WebApplication/SmartEstate.Contracts/*.csproj ./SmartEstate.Contracts/
COPY WebApplication/SmartEstate.DataAccess/*.csproj ./SmartEstate.DataAccess/
COPY WebApplication/SmartEstate.DatabaseContext/*.csproj ./SmartEstate.DatabaseContext/
COPY WebApplication/SmartEstate.DatabaseModel/*.csproj ./SmartEstate.DatabaseModel/
COPY WebApplication/SmartEstate.Email/*.csproj ./SmartEstate.Email/
COPY WebApplication/SmartEstate.Infrastructure/*.csproj ./SmartEstate.Infrastructure/
COPY WebApplication/SmartEstate.Routing/*.csproj ./SmartEstate.Routing/


RUN dotnet restore "WebApplication.sln"
COPY . .

RUN dotnet publish "WebApplication/SmartEstate.API/SmartEstate.API.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "SmartEstate.API.dll"]
